﻿/*
Insert initial data for OverallGradeLookup
*/

SET IDENTITY_INSERT [dbo].[OverallGradeLookup] ON

MERGE INTO [dbo].[OverallGradeLookup] AS Target 
USING (VALUES 
	-- Table 1 Start
	-- T Level in Design, Surveying and Planning for Construction
	(1, 1, 1, 10, 17, 1),
	(2, 1, 1, 11, 18, 1),
	(3, 1, 1, 12, 18, 1),

	(4, 1, 2, 10, 18, 1),
	(5, 1, 2, 11, 18, 1),
	(6, 1, 2, 12, 19, 1),

	(7, 1, 3, 10, 18, 1),
	(8, 1, 3, 11, 19, 1),
	(9, 1, 3, 12, 19, 1),

	(10, 1, 4, 10, 19, 1),
	(11, 1, 4, 11, 19, 1),
	(12, 1, 4, 12, 20, 1),

	(13, 1, 5, 10, 19, 1),
	(14, 1, 5, 11, 20, 1),
	(15, 1, 5, 12, 20, 1),

	(16, 1, 6, 10, 20, 1),
	(17, 1, 6, 11, 20, 1),
	(18, 1, 6, 12, 20, 1),

	-- T Level in Digital Production, Design and Development
	(19, 5, 1, 10, 17, 1),
	(20, 5, 1, 11, 18, 1),
	(21, 5, 1, 12, 18, 1),

	(22, 5, 2, 10, 18, 1),
	(23, 5, 2, 11, 18, 1),
	(24, 5, 2, 12, 19, 1),

	(25, 5, 3, 10, 18, 1),
	(26, 5, 3, 11, 19, 1),
	(27, 5, 3, 12, 19, 1),

	(28, 5, 4, 10, 19, 1),
	(29, 5, 4, 11, 19, 1),
	(30, 5, 4, 12, 20, 1),

	(31, 5, 5, 10, 19, 1),
	(32, 5, 5, 11, 20, 1),
	(33, 5, 5, 12, 20, 1),

	(34, 5, 6, 10, 20, 1),
	(35, 5, 6, 11, 20, 1),
	(36, 5, 6, 12, 20, 1),

	-- T Level in Digital Support Services
	(37, 6, 1, 10, 17, 1),
	(38, 6, 1, 11, 18, 1),
	(39, 6, 1, 12, 18, 1),

	(40, 6, 2, 10, 18, 1),
	(41, 6, 2, 11, 18, 1),
	(42, 6, 2, 12, 19, 1),

	(43, 6, 3, 10, 18, 1),
	(44, 6, 3, 11, 19, 1),
	(45, 6, 3, 12, 19, 1),

	(46, 6, 4, 10, 19, 1),
	(47, 6, 4, 11, 19, 1),
	(48, 6, 4, 12, 20, 1),

	(49, 6, 5, 10, 19, 1),
	(50, 6, 5, 11, 20, 1),
	(51, 6, 5, 12, 20, 1),

	(52, 6, 6, 10, 20, 1),
	(53, 6, 6, 11, 20, 1),
	(54, 6, 6, 12, 20, 1),

	-- T Level in Digital Business Services
	(55, 7, 1, 10, 17, 1),
	(56, 7, 1, 11, 18, 1),
	(57, 7, 1, 12, 18, 1),

	(58, 7, 2, 10, 18, 1),
	(59, 7, 2, 11, 18, 1),
	(60, 7, 2, 12, 19, 1),

	(61, 7, 3, 10, 18, 1),
	(62, 7, 3, 11, 19, 1),
	(63, 7, 3, 12, 19, 1),

	(64, 7, 4, 10, 19, 1),
	(65, 7, 4, 11, 19, 1),
	(66, 7, 4, 12, 20, 1),

	(67, 7, 5, 10, 19, 1),
	(68, 7, 5, 11, 20, 1),
	(69, 7, 5, 12, 20, 1),

	(70, 7, 6, 10, 20, 1),
	(71, 7, 6, 11, 20, 1),
	(72, 7, 6, 12, 20, 1),

	-- T Level in Health
	(73, 8, 1, 10, 17, 1),
	(74, 8, 1, 11, 18, 1),
	(75, 8, 1, 12, 18, 1),

	(76, 8, 2, 10, 18, 1),
	(77, 8, 2, 11, 18, 1),
	(78, 8, 2, 12, 19, 1),

	(79, 8, 3, 10, 18, 1),
	(80, 8, 3, 11, 19, 1),
	(81, 8, 3, 12, 19, 1),

	(82, 8, 4, 10, 19, 1),
	(83, 8, 4, 11, 19, 1),
	(84, 8, 4, 12, 20, 1),

	(85, 8, 5, 10, 19, 1),
	(86, 8, 5, 11, 20, 1),
	(87, 8, 5, 12, 20, 1),

	(88, 8, 6, 10, 20, 1),
	(89, 8, 6, 11, 20, 1),
	(90, 8, 6, 12, 20, 1),

	-- T Level in Healthcare Science
	(91, 9, 1, 10, 17, 1),
	(92, 9, 1, 11, 18, 1),
	(93, 9, 1, 12, 18, 1),

	(94, 9, 2, 10, 18, 1),
	(95, 9, 2, 11, 18, 1),
	(96, 9, 2, 12, 19, 1),

	(97, 9, 3, 10, 18, 1),
	(98, 9, 3, 11, 19, 1),
	(99, 9, 3, 12, 19, 1),

	(100, 9, 4, 10, 19, 1),
	(101, 9, 4, 11, 19, 1),
	(102, 9, 4, 12, 20, 1),

	(103, 9, 5, 10, 19, 1),
	(104, 9, 5, 11, 20, 1),
	(105, 9, 5, 12, 20, 1),

	(106, 9, 6, 10, 20, 1),
	(107, 9, 6, 11, 20, 1),
	(108, 9, 6, 12, 20, 1)

	-- Table 1 End	
  )
  AS Source ([Id], [TlPathwayId], [TlLookupCoreGradeId], [TlLookupSpecialismGradeId], [TlLookupOverallGradeId], [IsActive]) 
ON Target.[Id] = Source.[Id] 
-- Update from Source when Id is Matched
WHEN MATCHED 
	 AND ((Target.[TlPathwayId] <> Source.[TlPathwayId])
	   OR (Target.[TlLookupCoreGradeId] <> Source.[TlLookupCoreGradeId])
	   OR (Target.[TlLookupSpecialismGradeId] <> Source.[TlLookupSpecialismGradeId])
	   OR (Target.[TlLookupOverallGradeId] <> Source.[TlLookupOverallGradeId])
	   OR (Target.[IsActive] <> Source.[IsActive]))
THEN 
UPDATE SET 
	[TlPathwayId] = Source.[TlPathwayId],
	[TlLookupCoreGradeId] = Source.[TlLookupCoreGradeId],
	[TlLookupSpecialismGradeId] = Source.[TlLookupSpecialismGradeId],
	[TlLookupOverallGradeId] = Source.[TlLookupOverallGradeId],
	[IsActive] = Source.[IsActive],
	[ModifiedOn] = GETDATE(),
	[ModifiedBy] = 'System'
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [TlPathwayId], [TlLookupCoreGradeId], [TlLookupSpecialismGradeId], [TlLookupOverallGradeId], [IsActive], [CreatedBy]) 
	VALUES ([Id], [TlPathwayId], [TlLookupCoreGradeId], [TlLookupSpecialismGradeId], [TlLookupOverallGradeId], [IsActive], 'System') 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT [dbo].[OverallGradeLookup] OFF
