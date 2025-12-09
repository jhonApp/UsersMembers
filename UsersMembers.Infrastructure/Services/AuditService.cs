using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using UsersMembers.Application.Service.Audit;
using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Entities.Audit;
using UsersMembers.Domain.Interface;
using UsersMembers.Infrastructure.Services.Audit;

namespace UsersMembers.Infrastructure.Services
{
    internal abstract class AuditService : ServiceBase
    {
        private readonly IDynamoDBContext _context;

        protected readonly IEventRepository EventRepository;
        protected readonly string NomeUsuario;
        protected readonly IEnumerable<JsonConverter> JsonConvertersAuditoria;
        protected readonly ClaimsUser claimsUserLoggin;

        protected AuditService([NotNull] ServiceAuditDependence serviceAuditDependence)
            : base(serviceAuditDependence)
        {
            EventRepository = serviceAuditDependence.EventRepository;
            NomeUsuario = claimsUserLoggin?.NameAccess;

            JsonConvertersAuditoria = new JsonConverter[]
            {
                new DadosArquivoJsonConverter<DadosArquivo>(() => new DadosArquivo()),
                new DadosArquivoJsonConverter<RetornoObterArquivoProspeccao>(
                    () => new RetornoObterArquivoProspeccao()
                ),
                new DadosArquivoJsonConverter<RetornoObterGraficosRentabilidade>(
                    () => new RetornoObterGraficosRentabilidade()
                )
            };
        }

        public TRetorno AuditarOperacao<TRetorno>(
            [NotNull] LogWrapper logWrapper,
            string descricaoEvento,
            [NotNull] Func<TRetorno> operacao,
            object entradaOperacao,
            [CallerMemberName] string callerMemberName = "")
        {
            return AuditarOperacao(new PayloadAuditOperation<TRetorno>()
            {
                LogWrapper = logWrapper,
                DescricaoEvento = descricaoEvento,
                Operacao = operacao,
                EntradaOperacao = entradaOperacao
            }, callerMemberName);
        }

        public TRetorno AuditarOperacao<TRetorno>(
            [NotNull] PayloadAuditOperation<TRetorno> parametrosAuditoria,
            [CallerMemberName] string callerMemberName = "")
        {
            Event eventoAuditoria = null;
            List<MetadataEvent> metadadosEvento = new();
            TRetorno retornoOperacao = default;
            DateTime dthEvento = DateTime.Now;
            Guid idEvento = Guid.NewGuid();

            try
            {
                string valorMetadadoEntradaOperacao = JsonSerializer.Serialize(parametrosAuditoria.EntradaOperacao);

                metadadosEvento.Add(new MetadataEvent()
                {
                    Id = Guid.NewGuid(),
                    IdEvent = idEvento,
                    DateHourMetadata = dthEvento.AddTicks(1),
                    NameKey = "EntradaOperacao",
                    ValueMetadata = valorMetadadoEntradaOperacao
                });

                Stopwatch timer = Stopwatch.StartNew();
                retornoOperacao = parametrosAuditoria.LogWrapper.LogOperation(
                    parametrosAuditoria.Operacao,
                    callerMemberName
                );
                timer.Stop();

                metadadosEvento.Add(new MetadataEvent()
                {
                    Id = Guid.NewGuid(),
                    IdEvent = idEvento,
                    DateHourMetadata = dthEvento.AddTicks(2),
                    NameKey = "TempoDuracaoExecucao_Milisegundos",
                    ValueMetadata = timer.ElapsedMilliseconds.ToString("###0")
                });

                bool operacaoComSucesso = RecuperarStatusSucessoOperacao(retornoOperacao);

                eventoAuditoria = new Event()
                {
                    Id = idEvento,
                    IdTipoEvento = (operacaoComSucesso ? EnumTipoEvento.RequisicaoApi : EnumTipoEvento.Falha).ObterCodigoNumerico(),
                    NomeFuncionalidade = callerMemberName,
                    Descricao = parametrosAuditoria.DescricaoEvento,
                    IdSessao = claimsUserLoggin?.IdSession,
                    NomeAcessoUsuario = ObterNomeUsuarioAuditado(parametrosAuditoria.LogWrapper),
                    DataHoraOcorrencia = dthEvento,
                    Metadados = metadadosEvento
                };

                var serializerOptions = ConfigurarOpcoesSerializacao();
                string valorMetadadoRetornoOperacao = JsonSerializer.Serialize(retornoOperacao, serializerOptions);
                metadadosEvento.Add(new MetadataEvent()
                {
                    Id = Guid.NewGuid(),
                    IdEvent = idEvento,
                    DateHourMetadata = DateTime.Now,
                    NameKey = "RetornoOperacao",
                    ValueMetadata = valorMetadadoRetornoOperacao
                });
            }
            catch (Exception exc)
            {
                if (eventoAuditoria == null)
                    eventoAuditoria = new Event()
                    {
                        Id = Guid.NewGuid(),
                        Descricao = parametrosAuditoria.DescricaoEvento,
                        DataHoraOcorrencia = DateTime.Now,
                        Metadados = metadadosEvento,
                        NomeAcessoUsuario = ObterNomeUsuarioAuditado(parametrosAuditoria.LogWrapper),
                        NomeFuncionalidade = callerMemberName,
                        IdSessao = claimsUserLoggin?.IdSession
                    };

                eventoAuditoria.IdTipoEvento = EnumTipoEvento.Falha.ObterCodigoNumerico();

                string detalhesExcecao = exc.ToString();

                metadadosEvento.Add(new()
                {
                    Id = Guid.NewGuid(),
                    IdEvent = eventoAuditoria.Id,
                    DateHourMetadata = DateTime.Now,
                    NameKey = "Excecao",
                    ValueMetadata = detalhesExcecao
                });

                parametrosAuditoria.LogWrapper.LogError(exc.Message, exc);

                if (parametrosAuditoria.ErrorBoundaryOperacao is null)
                    throw;

                retornoOperacao = parametrosAuditoria.ErrorBoundaryOperacao(exc);
                string valorMetadadoRetornoOperacao = JsonSerializer.Serialize(retornoOperacao);
                metadadosEvento.Add(new MetadataEvent()
                {
                    Id = Guid.NewGuid(),
                    IdEvent = idEvento,
                    DateHourMetadata = DateTime.Now,
                    NameKey = "RetornoOperacao",
                    ValueMetadata = valorMetadadoRetornoOperacao
                });
            }
            finally
            {
                EventoRepository.Criar(eventoAuditoria);
            }

            return retornoOperacao;
        }

        protected JsonSerializerOptions ConfigurarOpcoesSerializacao()
        {
            var serializerOptions = new JsonSerializerOptions();
            foreach (var converter in JsonConvertersAuditoria)
            {
                serializerOptions.Converters.Add(converter);
            }

            return serializerOptions;
        }

        protected static bool RecuperarStatusSucessoOperacao<TRetorno>(TRetorno retornoOperacao)
        {
            bool operacaoComSucesso = true;
            if (retornoOperacao is RetornoOperacao)
                operacaoComSucesso = (retornoOperacao as RetornoOperacao).Sucesso;
            return operacaoComSucesso;
        }

        protected string ObterNomeUsuarioAuditado([NotNull] LogWrapper logWrapper)
        {
            string nomeAcessoUsuario = NomeUsuario;
            if (string.IsNullOrWhiteSpace(nomeAcessoUsuario))
                if (!string.IsNullOrWhiteSpace(logWrapper.LogDefaultProperties?.UserId))
                    nomeAcessoUsuario = logWrapper.LogDefaultProperties.UserId;
                else
                    nomeAcessoUsuario = "Anônimo";

            return nomeAcessoUsuario;
        }
    }
}
