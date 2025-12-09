using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsersMembers.Infrastructure.Services.Audit
{
    internal class PayloadAuditOperation<TRetorno>
    {
        public LogWrapper LogWrapper { get; set; }
        public string DescricaoEvento { get; set; }
        public Func<TRetorno> Operacao { get; set; }
        public object EntradaOperacao { get; set; }
        public Func<Exception, TRetorno> ErrorBoundaryOperacao { get; set; }

        public PayloadAuditOperation() { }

        public PayloadAuditOperation(
            LogWrapper logWrapper,
            string descricaoEvento,
            Func<TRetorno> operacao,
            object entradaOperacao,
            Func<Exception, TRetorno> errorBoundaryOperacao)
        {
            LogWrapper = logWrapper;
            DescricaoEvento = descricaoEvento;
            Operacao = operacao;
            EntradaOperacao = entradaOperacao;
            ErrorBoundaryOperacao = errorBoundaryOperacao;
        }
    }
}
