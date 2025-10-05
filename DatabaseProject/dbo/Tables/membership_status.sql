CREATE TABLE [dbo].[membership_status] (
    [status_id]   INT           IDENTITY (1, 1) NOT NULL,
    [status_name] NVARCHAR (20) NOT NULL,
    PRIMARY KEY CLUSTERED ([status_id] ASC),
    UNIQUE NONCLUSTERED ([status_name] ASC)
);

