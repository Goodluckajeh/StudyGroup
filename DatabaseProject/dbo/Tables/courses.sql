CREATE TABLE [dbo].[Courses] (
    [CourseId]    INT            IDENTITY (1, 1) NOT NULL,
    [CourseCode]  NVARCHAR (50)  NOT NULL,
    [CourseName]  NVARCHAR (255) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([CourseId] ASC)
);

