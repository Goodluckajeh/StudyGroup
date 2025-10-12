CREATE TABLE [dbo].[DaysOfWeek] (
    [DayId]   INT           IDENTITY (1, 1) NOT NULL,
    [DayName] NVARCHAR (10) NOT NULL,
    PRIMARY KEY CLUSTERED ([DayId] ASC),
    UNIQUE NONCLUSTERED ([DayName] ASC)
);

