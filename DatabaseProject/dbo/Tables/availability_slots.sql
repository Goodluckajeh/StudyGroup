CREATE TABLE [dbo].[availability_slots] (
    [availability_id] INT      IDENTITY (1, 1) NOT NULL,
    [user_id]         INT      NOT NULL,
    [day_id]          INT      NOT NULL,
    [start_time]      TIME (7) NOT NULL,
    [end_time]        TIME (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([availability_id] ASC),
    CONSTRAINT [FK_AvailabilitySlots_Days] FOREIGN KEY ([day_id]) REFERENCES [dbo].[days_of_week] ([day_id]),
    CONSTRAINT [FK_AvailabilitySlots_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([user_id]) ON DELETE CASCADE
);

