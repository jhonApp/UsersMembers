# Async Ingestion Infrastructure Setup

This guide explains how to set up the AWS resources required for the Async Ingestion "Sweet Spot" architecture.

## Architecture Overview
1.  **Application** sends event to **SQS**.
2.  **Ingestion Lambda** reads SQS and writes to **DynamoDB**.
3.  **DynamoDB** has **TTL** enabled (90 days).
4.  **DynamoDB Stream** triggers **Archival Lambda**.
5.  **Archival Lambda** saves deleted items to **S3**.

## 1. SQS Queue
- **Name**: `EventsQueue` (or any name, update `appsettings.json` or code).
- **Type**: Standard.
- **Visibility Timeout**: > Lambda Timeout (e.g., 60s).

## 2. DynamoDB Table
- **Name**: `EventsTable`.
- **Partition Key**: `Id` (String).
- **TTL Attribute**: `ttl` (Number). Enabled.
- **Stream**: `New and Old Images` (or just Old Images, but Archiver needs `OldImage`). Use **ViewType**: `KEYS_ONLY` | `NEW_IMAGE` | `OLD_IMAGE` | `NEW_AND_OLD_IMAGES`. **MUST BE** `NEW_AND_OLD_IMAGES` or `OLD_IMAGE`.

## 3. S3 Bucket
- **Name**: `user-members-archive` (Must be globally unique).
- **Permissions**: Ensure Lambdas can write to it.

## 4. Lambda Functions

### Ingestion Function (`DynamoDbIngestionHandler`)
- **Trigger**: SQS (`EventsQueue`).
- **Environment Variables**:
    - `DYNAMODB_TABLE_NAME`: `EventsTable`
- **Permissions**: `sqs:ReceiveMessage`, `sqs:DeleteMessage`, `dynamodb:PutItem`.

### Archival Function (`DynamoDbStreamArchiver`)
- **Trigger**: DynamoDB Stream (`EventsTable`).
- **Batch Size**: 1-100 (depending on volume).
- **Environment Variables**:
    - `ARCHIVE_BUCKET_NAME`: `user-members-archive`
- **Permissions**: `dynamodb:GetRecords`, `dynamodb:GetShardIterator`, `dynamodb:DescribeStream`, `dynamodb:ListStreams`, `s3:PutObject`.

## 5. App Configuration
Ensure your Application has `AWS_REGION` and credentials configured.
Inject `IEventProducer` and call `PublishAsync` with the Queue URL.
