CREATE TABLE [dbo].[Conversations] (
    [ConversationId]   INT           IDENTITY (1, 1) NOT NULL,
    [ConversationType] NVARCHAR (20) NOT NULL,
    [StudyGroupId]     INT           NULL,
    [CreatedAt]        DATETIME2 (7) DEFAULT (getutcdate()) NOT NULL,
    [LastMessageAt]    DATETIME2 (7) NULL,
    [IsActive]         BIT           DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([ConversationId] ASC),
    CHECK ([ConversationType]='GroupChat' OR [ConversationType]='DirectMessage'),
    CONSTRAINT [CK_Conversation_Type] CHECK ([ConversationType]='GroupChat' AND [StudyGroupId] IS NOT NULL OR [ConversationType]='DirectMessage' AND [StudyGroupId] IS NULL),
    FOREIGN KEY ([StudyGroupId]) REFERENCES [dbo].[StudyGroups] ([GroupId]) ON DELETE CASCADE
);

