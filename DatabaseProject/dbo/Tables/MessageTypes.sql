CREATE TABLE [dbo].[MessageTypes] (
    [MessageTypeId] INT            IDENTITY (1, 1) NOT NULL,
    [TypeName]      NVARCHAR (50)  NOT NULL,
    [Description]   NVARCHAR (200) NULL,
    PRIMARY KEY CLUSTERED ([MessageTypeId] ASC),
    UNIQUE NONCLUSTERED ([TypeName] ASC)
);

