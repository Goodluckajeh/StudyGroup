CREATE TABLE [dbo].[GroupMembers] (
    [GroupMemberId] INT IDENTITY (1, 1) NOT NULL,
    [GroupId]       INT NOT NULL,
    [UserId]        INT NOT NULL,
    [StatusId]      INT NOT NULL,
    PRIMARY KEY CLUSTERED ([GroupMemberId] ASC),
    CONSTRAINT [FK_GroupMembers_Groups] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[StudyGroups] ([GroupId]) ON DELETE CASCADE,
    CONSTRAINT [FK_GroupMembers_Status] FOREIGN KEY ([StatusId]) REFERENCES [dbo].[MembershipStatus] ([StatusId]),
    CONSTRAINT [FK_GroupMembers_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]) ON DELETE CASCADE
);

