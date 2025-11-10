CREATE TABLE [dbo].[ConversationParticipants] (
    [ParticipantId]  INT           IDENTITY (1, 1) NOT NULL,
    [ConversationId] INT           NOT NULL,
    [UserId]         INT           NOT NULL,
    [JoinedAt]       DATETIME2 (7) DEFAULT (getutcdate()) NOT NULL,
    [LastReadAt]     DATETIME2 (7) NULL,
    [IsActive]       BIT           DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ParticipantId] ASC),
    FOREIGN KEY ([ConversationId]) REFERENCES [dbo].[Conversations] ([ConversationId]) ON DELETE CASCADE,
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    UNIQUE NONCLUSTERED ([ConversationId] ASC, [UserId] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ConversationParticipants_User]
    ON [dbo].[ConversationParticipants]([UserId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ConversationParticipants_Conversation]
    ON [dbo].[ConversationParticipants]([ConversationId] ASC);

