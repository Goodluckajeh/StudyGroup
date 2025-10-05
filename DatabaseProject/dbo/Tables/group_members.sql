CREATE TABLE [dbo].[group_members] (
    [group_member_id] INT IDENTITY (1, 1) NOT NULL,
    [group_id]        INT NOT NULL,
    [user_id]         INT NOT NULL,
    [status_id]       INT NOT NULL,
    PRIMARY KEY CLUSTERED ([group_member_id] ASC),
    CONSTRAINT [FK_GroupMembers_Groups] FOREIGN KEY ([group_id]) REFERENCES [dbo].[study_groups] ([group_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GroupMembers_Status] FOREIGN KEY ([status_id]) REFERENCES [dbo].[membership_status] ([status_id]),
    CONSTRAINT [FK_GroupMembers_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([user_id]) ON DELETE CASCADE
);

