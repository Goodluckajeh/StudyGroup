CREATE TABLE [dbo].[availability_slots] (
    [availability_id] INT          IDENTITY (1, 1) NOT NULL,
    [user_id]         INT          NOT NULL,
    [day_of_week]     NVARCHAR (3) NOT NULL,
    [start_time]      TIME (7)     NOT NULL,
    [end_time]        TIME (7)     NOT NULL,
    PRIMARY KEY CLUSTERED ([availability_id] ASC),
    CHECK ([day_of_week]='Sun' OR [day_of_week]='Sat' OR [day_of_week]='Fri' OR [day_of_week]='Thu' OR [day_of_week]='Wed' OR [day_of_week]='Tue' OR [day_of_week]='Mon'),
    CONSTRAINT [FK_AvailabilitySlots_Users] FOREIGN KEY ([user_id]) REFERENCES [dbo].[users] ([user_id]) ON DELETE CASCADE
);

