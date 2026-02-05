CREATE TABLE [dbo].[MembershipStatus] (
    [StatusId]   INT           IDENTITY (1, 1) NOT NULL,
    [StatusName] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([StatusId] ASC),
    UNIQUE NONCLUSTERED ([StatusName] ASC)
);

