CREATE TABLE [dbo].[days_of_week] (
    [day_id]   INT           IDENTITY (1, 1) NOT NULL,
    [day_name] NVARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([day_id] ASC),
    UNIQUE NONCLUSTERED ([day_name] ASC)
);

