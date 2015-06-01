CREATE TABLE [dbo].[DB_Version] (
    [ID]                 INT          IDENTITY (1, 1) NOT NULL,
    [MajorReleaseNumber] INT          NOT NULL,
    [MinorReleaseNumber] INT          NOT NULL,
    [PointReleaseNumber] INT          NOT NULL,
    [ScriptName]         VARCHAR (50) NOT NULL,
    [DateApplied]        DATETIME     NOT NULL,
    CONSTRAINT [PK_SchemaChangeLog] PRIMARY KEY CLUSTERED ([ID] ASC)
);





