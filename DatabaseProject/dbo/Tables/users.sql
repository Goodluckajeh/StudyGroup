CREATE TABLE [dbo].[users] (
    [user_id]       INT            IDENTITY (1, 1) NOT NULL,
    [name]          NVARCHAR (255) NOT NULL,
    [email]         NVARCHAR (255) NOT NULL,
    [password_hash] NVARCHAR (255) NOT NULL,
    [skills]        NVARCHAR (MAX) NULL,
    [visibility]    BIT            DEFAULT ((1)) NULL,
    [bio]           NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([user_id] ASC),
    UNIQUE NONCLUSTERED ([email] ASC)
);

