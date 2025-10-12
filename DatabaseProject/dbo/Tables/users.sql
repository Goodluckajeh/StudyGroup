CREATE TABLE [dbo].[Users] (
    [UserId]       INT            IDENTITY (1, 1) NOT NULL,
    [FirstName]    NVARCHAR (255) NOT NULL,
    [LastName]     NVARCHAR (255) NOT NULL,
    [Email]        NVARCHAR (255) NOT NULL,
    [PasswordHash] NVARCHAR (255) NOT NULL,
    [Skills]       NVARCHAR (MAX) NULL,
    [Visibility]   BIT            DEFAULT ((1)) NULL,
    [Bio]          NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

