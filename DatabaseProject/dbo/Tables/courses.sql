CREATE TABLE [dbo].[courses] (
    [course_id]   INT            IDENTITY (1, 1) NOT NULL,
    [course_code] NVARCHAR (50)  NOT NULL,
    [course_name] NVARCHAR (255) NOT NULL,
    [description] NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([course_id] ASC)
);

