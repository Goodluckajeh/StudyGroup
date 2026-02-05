CREATE TABLE [dbo].[Messages] (
    [MessageId]        INT            IDENTITY (1, 1) NOT NULL,
    [ConversationId]   INT            NOT NULL,
    [SenderId]         INT            NOT NULL,
    [MessageTypeId]    INT            NOT NULL,
    [Content]          NVARCHAR (MAX) NULL,
    [MediaUrl]         NVARCHAR (500) NULL,
    [MediaFileName]    NVARCHAR (255) NULL,
    [MediaFileSize]    BIGINT         NULL,
    [ThumbnailUrl]     NVARCHAR (500) NULL,
    [SentAt]           DATETIME2 (7)  DEFAULT (getutcdate()) NOT NULL,
    [EditedAt]         DATETIME2 (7)  NULL,
    [IsDeleted]        BIT            DEFAULT ((0)) NOT NULL,
    [DeletedAt]        DATETIME2 (7)  NULL,
    [ReplyToMessageId] INT            NULL,
    PRIMARY KEY CLUSTERED ([MessageId] ASC),
    FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([ConversationId]) ON DELETE CASCADE,
    FOREIGN KEY ([MessageTypeId]) REFERENCES [dbo].[MessageTypes] ([MessageTypeId]),
    FOREIGN KEY ([ReplyToMessageId]) REFERENCES [dbo].[Messages] ([MessageId]),
    FOREIGN KEY ([SenderId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_Conversation_SentAt]
    ON [dbo].[Messages]([ConversationId] ASC, [SentAt] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_Sender]
    ON [dbo].[Messages]([SenderId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Messages_ReplyTo]
    ON [dbo].[Messages]([ReplyToMessageId] ASC);

