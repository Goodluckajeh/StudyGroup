CREATE TABLE [dbo].[study_groups] (
    [group_id]    INT            IDENTITY (1, 1) NOT NULL,
    [course_id]   INT            NOT NULL,
    [creator_id]  INT            NOT NULL,
    [topic]       NVARCHAR (255) NULL,
    [time_slot]   NVARCHAR (MAX) NULL,
    [description] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([group_id] ASC),
    CONSTRAINT [FK_StudyGroups_Courses] FOREIGN KEY ([course_id]) REFERENCES [dbo].[courses] ([course_id]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudyGroups_Users] FOREIGN KEY ([creator_id]) REFERENCES [dbo].[users] ([user_id])
);

