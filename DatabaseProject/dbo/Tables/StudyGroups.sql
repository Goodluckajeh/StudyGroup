CREATE TABLE [dbo].[StudyGroups] (
    [GroupId]     INT            IDENTITY (1, 1) NOT NULL,
    [CourseId]    INT            NOT NULL,
    [CreatorId]   INT            NOT NULL,
    [Topic]       NVARCHAR (255) NULL,
    [TimeSlot]    NVARCHAR (MAX) NULL,
    [Description] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([GroupId] ASC),
    CONSTRAINT [FK_StudyGroups_Courses] FOREIGN KEY ([CourseId]) REFERENCES [dbo].[Courses] ([CourseId]) ON DELETE CASCADE,
    CONSTRAINT [FK_StudyGroups_Users] FOREIGN KEY ([CreatorId]) REFERENCES [dbo].[Users] ([UserId])
);

