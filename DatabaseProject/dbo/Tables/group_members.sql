CREATE TABLE [dbo].[group_members] (
    [group_member_id] INT           IDENTITY (1, 1) NOT NULL,
    [group_id]        INT           NOT NULL,
    [user_id]         INT           NOT NULL,
    [status]          NVARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([group_member_id] ASC),
    CHECK ([status]='rejected' OR [status]='approved' OR [status]='pending'),
    CONSTRAINT [FK_GroupMembers_Groups] FOREIGN KEY ([group_id]) REFERENCES [dbo].[study_groups] ([group_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_GroupMembers_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([user_id]) ON DELETE CASCADE
);

