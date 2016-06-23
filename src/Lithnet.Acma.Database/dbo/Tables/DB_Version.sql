CREATE TABLE [dbo].[DB_Version] (
    [ID]                 INT          IDENTITY (1, 1) NOT NULL,
    [MajorReleaseNumber] VARCHAR (2)  NOT NULL,
    [MinorReleaseNumber] VARCHAR (2)  NOT NULL,
    [PointReleaseNumber] VARCHAR (4)  NOT NULL,
    [ScriptName]         VARCHAR (50) NOT NULL,
    [DateApplied]        DATETIME     NOT NULL,
    CONSTRAINT [PK_SchemaChangeLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);

