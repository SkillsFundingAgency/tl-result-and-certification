﻿/*
Insert initial data for OverallGradeLookup
*/

SET IDENTITY_INSERT [dbo].[OverallGradeLookup] ON

MERGE INTO [dbo].[OverallGradeLookup] AS Target 
USING (VALUES 
	-- Table 1 Start
	-- T Level in Design, Surveying and Planning for Construction
	(181, 1, 1, 10, 17, 1),
	(182, 1, 1, 11, 18, 1),
	(183, 1, 1, 12, 18, 1),

	(184, 1, 2, 10, 18, 1),
	(185, 1, 2, 11, 18, 1),
	(186, 1, 2, 12, 19, 1),
	
	(187, 1, 3, 10, 18, 1),
	(188, 1, 3, 11, 19, 1),
	(189, 1, 3, 12, 19, 1),
	
	(190, 1, 4, 10, 18, 1),
	(191, 1, 4, 11, 19, 1),
	(192, 1, 4, 12, 20, 1),
	
	(193, 1, 5, 10, 19, 1),
	(194, 1, 5, 11, 19, 1),
	(195, 1, 5, 12, 20, 1),
	
	(196, 1, 6, 10, 19, 1),
	(197, 1, 6, 11, 20, 1),
	(198, 1, 6, 12, 20, 1),
	
	-- T Level in Digital Production, Design and Development
	(199, 5, 1, 10, 17, 1),
	(200, 5, 1, 11, 18, 1),
	(201, 5, 1, 12, 18, 1),

	(202, 5, 2, 10, 18, 1),
	(203, 5, 2, 11, 18, 1),
	(204, 5, 2, 12, 19, 1),

	(205, 5, 3, 10, 18, 1),
	(206, 5, 3, 11, 19, 1),
	(207, 5, 3, 12, 19, 1),

	(208, 5, 4, 10, 18, 1),
	(209, 5, 4, 11, 19, 1),
	(210, 5, 4, 12, 20, 1),

	(211, 5, 5, 10, 19, 1),
	(212, 5, 5, 11, 19, 1),
	(213, 5, 5, 12, 20, 1),

	(214, 5, 6, 10, 19, 1),
	(215, 5, 6, 11, 20, 1),
	(216, 5, 6, 12, 20, 1),

	-- T Level in Digital Support Services
	(217, 6, 1, 10, 17, 1),
	(218, 6, 1, 11, 18, 1),
	(219, 6, 1, 12, 18, 1),

	(220, 6, 2, 10, 18, 1),
	(221, 6, 2, 11, 18, 1),
	(222, 6, 2, 12, 19, 1),

	(223, 6, 3, 10, 18, 1),
	(224, 6, 3, 11, 19, 1),
	(225, 6, 3, 12, 19, 1),

	(226, 6, 4, 10, 18, 1),
	(227, 6, 4, 11, 19, 1),
	(228, 6, 4, 12, 20, 1),

	(229, 6, 5, 10, 19, 1),
	(230, 6, 5, 11, 19, 1),
	(231, 6, 5, 12, 20, 1),

	(232, 6, 6, 10, 19, 1),
	(233, 6, 6, 11, 20, 1),
	(234, 6, 6, 12, 20, 1),

	-- T Level in Digital Business Services
	(235, 7, 1, 10, 17, 1),
	(236, 7, 1, 11, 18, 1),
	(237, 7, 1, 12, 18, 1),

	(238, 7, 2, 10, 18, 1),
	(239, 7, 2, 11, 18, 1),
	(240, 7, 2, 12, 19, 1),

	(241, 7, 3, 10, 18, 1),
	(242, 7, 3, 11, 19, 1),
	(243, 7, 3, 12, 19, 1),

	(244, 7, 4, 10, 18, 1),
	(245, 7, 4, 11, 19, 1),
	(246, 7, 4, 12, 20, 1),

	(247, 7, 5, 10, 19, 1),
	(248, 7, 5, 11, 19, 1),
	(249, 7, 5, 12, 20, 1),

	(250, 7, 6, 10, 19, 1),
	(251, 7, 6, 11, 20, 1),
	(252, 7, 6, 12, 20, 1),

	-- T Level in Health
	(253, 8, 1, 10, 17, 1),
	(254, 8, 1, 11, 18, 1),
	(255, 8, 1, 12, 18, 1),

	(256, 8, 2, 10, 18, 1),
	(257, 8, 2, 11, 18, 1),
	(258, 8, 2, 12, 19, 1),

	(259, 8, 3, 10, 18, 1),
	(260, 8, 3, 11, 19, 1),
	(261, 8, 3, 12, 19, 1),

	(262, 8, 4, 10, 18, 1),
	(263, 8, 4, 11, 19, 1),
	(264, 8, 4, 12, 20, 1),

	(265, 8, 5, 10, 19, 1),
	(266, 8, 5, 11, 19, 1),
	(267, 8, 5, 12, 20, 1),

	(268, 8, 6, 10, 19, 1),
	(269, 8, 6, 11, 20, 1),
	(270, 8, 6, 12, 20, 1),

	-- T Level in Healthcare Science
	(271, 9, 1, 10, 17, 1),
	(272, 9, 1, 11, 18, 1),
	(273, 9, 1, 12, 18, 1),

	(274, 9, 2, 10, 18, 1),
	(275, 9, 2, 11, 18, 1),
	(276, 9, 2, 12, 19, 1),

	(277, 9, 3, 10, 18, 1),
	(278, 9, 3, 11, 19, 1),
	(279, 9, 3, 12, 19, 1),

	(280, 9, 4, 10, 18, 1),
	(281, 9, 4, 11, 19, 1),
	(282, 9, 4, 12, 20, 1),

	(283, 9, 5, 10, 19, 1),
	(284, 9, 5, 11, 19, 1),
	(285, 9, 5, 12, 20, 1),

	(286, 9, 6, 10, 19, 1),
	(287, 9, 6, 11, 20, 1),
	(288, 9, 6, 12, 20, 1),

	-- Table 1 End	

	-- Table 2 Start

	-- T Level in Onsite Construction
	(289, 2, 1, 10, 17, 1),
	(290, 2, 1, 11, 18, 1),
	(291, 2, 1, 12, 18, 1),

	(292, 2, 2, 10, 18, 1),
	(293, 2, 2, 11, 18, 1),
	(294, 2, 2, 12, 19, 1),

	(295, 2, 3, 10, 18, 1),
	(296, 2, 3, 11, 19, 1),
	(297, 2, 3, 12, 19, 1),

	(298, 2, 4, 10, 18, 1),
	(299, 2, 4, 11, 19, 1),
	(300, 2, 4, 12, 20, 1),

	(301, 2, 5, 10, 19, 1),
	(302, 2, 5, 11, 19, 1),
	(303, 2, 5, 12, 20, 1),

	(304, 2, 6, 10, 19, 1),
	(305, 2, 6, 11, 20, 1),
	(306, 2, 6, 12, 20, 1),

	-- T Level in Building Services Engineering for Construction
	(307, 3, 1, 10, 17, 1),
	(308, 3, 1, 11, 18, 1),
	(309, 3, 1, 12, 18, 1),

	(310, 3, 2, 10, 18, 1),
	(311, 3, 2, 11, 18, 1),
	(312, 3, 2, 12, 19, 1),

	(313, 3, 3, 10, 18, 1),
	(314, 3, 3, 11, 19, 1),
	(315, 3, 3, 12, 19, 1),

	(316, 3, 4, 10, 18, 1),
	(317, 3, 4, 11, 19, 1),
	(318, 3, 4, 12, 20, 1),

	(319, 3, 5, 10, 19, 1),
	(320, 3, 5, 11, 19, 1),
	(321, 3, 5, 12, 20, 1),

	(322, 3, 6, 10, 19, 1),
	(323, 3, 6, 11, 20, 1),
	(324, 3, 6, 12, 20, 1),

	-- T Level in Education and Childcare
	(325, 4, 1, 10, 17, 1),
	(326, 4, 1, 11, 18, 1),
	(327, 4, 1, 12, 18, 1),

	(328, 4, 2, 10, 18, 1),
	(329, 4, 2, 11, 18, 1),
	(330, 4, 2, 12, 19, 1),

	(331, 4, 3, 10, 18, 1),
	(332, 4, 3, 11, 19, 1),
	(333, 4, 3, 12, 19, 1),

	(334, 4, 4, 10, 18, 1),
	(335, 4, 4, 11, 19, 1),
	(336, 4, 4, 12, 20, 1),

	(337, 4, 5, 10, 19, 1),
	(338, 4, 5, 11, 19, 1),
	(339, 4, 5, 12, 20, 1),

	(340, 4, 6, 10, 19, 1),
	(341, 4, 6, 11, 20, 1),
	(342, 4, 6, 12, 20, 1),

	-- T Level in Science
	(343, 10, 1, 10, 17, 1),
	(344, 10, 1, 11, 18, 1),
	(345, 10, 1, 12, 18, 1),

	(346, 10, 2, 10, 18, 1),
	(347, 10, 2, 11, 18, 1),
	(348, 10, 2, 12, 19, 1),

	(349, 10, 3, 10, 18, 1),
	(350, 10, 3, 11, 19, 1),
	(351, 10, 3, 12, 19, 1),

	(352, 10, 4, 10, 18, 1),
	(353, 10, 4, 11, 19, 1),
	(354, 10, 4, 12, 20, 1),

	(355, 10, 5, 10, 19, 1),
	(356, 10, 5, 11, 19, 1),
	(357, 10, 5, 12, 20, 1),

	(358, 10, 6, 10, 19, 1),
	(359, 10, 6, 11, 20, 1),
	(360, 10, 6, 12, 20, 1),

	-- T Level in Finance
	(361, 11, 1, 10, 17, 1),
	(362, 11, 1, 11, 18, 1),
	(363, 11, 1, 12, 18, 1),
	(364, 11, 2, 10, 18, 1),
	(365, 11, 2, 11, 18, 1),
	(366, 11, 2, 12, 19, 1),
	(367, 11, 3, 10, 18, 1),
	(368, 11, 3, 11, 19, 1),
	(369, 11, 3, 12, 19, 1),
	(370, 11, 4, 10, 18, 1),
	(371, 11, 4, 11, 19, 1),
	(372, 11, 4, 12, 20, 1),
	(373, 11, 5, 10, 19, 1),
	(374, 11, 5, 11, 19, 1),
	(375, 11, 5, 12, 20, 1),
	(376, 11, 6, 10, 19, 1),
	(377, 11, 6, 11, 20, 1),
	(378, 11, 6, 12, 20, 1),

	-- T Level in Accounting
	(379, 12, 1, 10, 17, 1),
	(380, 12, 1, 11, 18, 1),
	(381, 12, 1, 12, 18, 1),
	(382, 12, 2, 10, 18, 1),
	(383, 12, 2, 11, 18, 1),
	(384, 12, 2, 12, 19, 1),
	(385, 12, 3, 10, 18, 1),
	(386, 12, 3, 11, 19, 1),
	(387, 12, 3, 12, 19, 1),
	(388, 12, 4, 10, 18, 1),
	(389, 12, 4, 11, 19, 1),
	(390, 12, 4, 12, 20, 1),
	(391, 12, 5, 10, 19, 1),
	(392, 12, 5, 11, 19, 1),
	(393, 12, 5, 12, 20, 1),
	(394, 12, 6, 10, 19, 1),
	(395, 12, 6, 11, 20, 1),
	(396, 12, 6, 12, 20, 1),

	-- T Level in Design and Development for Engineering and Manufacturing
	(397, 13, 1, 10, 17, 1),
	(398, 13, 1, 11, 18, 1),
	(399, 13, 1, 12, 18, 1),
	(400, 13, 2, 10, 18, 1),
	(401, 13, 2, 11, 18, 1),
	(402, 13, 2, 12, 19, 1),
	(403, 13, 3, 10, 18, 1),
	(404, 13, 3, 11, 19, 1),
	(405, 13, 3, 12, 19, 1),
	(406, 13, 4, 10, 18, 1),
	(407, 13, 4, 11, 19, 1),
	(408, 13, 4, 12, 20, 1),
	(409, 13, 5, 10, 19, 1),
	(410, 13, 5, 11, 19, 1),
	(411, 13, 5, 12, 20, 1),
	(412, 13, 6, 10, 19, 1),
	(413, 13, 6, 11, 20, 1),
	(414, 13, 6, 12, 20, 1),

	-- T Level in Maintenance, Installation and Repair for Engineering and Manufacturing
	(415, 14, 1, 10, 17, 1),
	(416, 14, 1, 11, 18, 1),
	(417, 14, 1, 12, 18, 1),
	(418, 14, 2, 10, 18, 1),
	(419, 14, 2, 11, 18, 1),
	(420, 14, 2, 12, 19, 1),
	(421, 14, 3, 10, 18, 1),
	(422, 14, 3, 11, 19, 1),
	(423, 14, 3, 12, 19, 1),
	(424, 14, 4, 10, 18, 1),
	(425, 14, 4, 11, 19, 1),
	(426, 14, 4, 12, 20, 1),
	(427, 14, 5, 10, 19, 1),
	(428, 14, 5, 11, 19, 1),
	(429, 14, 5, 12, 20, 1),
	(430, 14, 6, 10, 19, 1),
	(431, 14, 6, 11, 20, 1),
	(432, 14, 6, 12, 20, 1),

	-- T Level in Engineering, Manufacturing, Processing and Control
	(433, 15, 1, 10, 17, 1),
	(434, 15, 1, 11, 18, 1),
	(435, 15, 1, 12, 18, 1),
	(436, 15, 2, 10, 18, 1),
	(437, 15, 2, 11, 18, 1),
	(438, 15, 2, 12, 19, 1),
	(439, 15, 3, 10, 18, 1),
	(440, 15, 3, 11, 19, 1),
	(441, 15, 3, 12, 19, 1),
	(442, 15, 4, 10, 18, 1),
	(443, 15, 4, 11, 19, 1),
	(444, 15, 4, 12, 20, 1),
	(445, 15, 5, 10, 19, 1),
	(446, 15, 5, 11, 19, 1),
	(447, 15, 5, 12, 20, 1),
	(448, 15, 6, 10, 19, 1),
	(449, 15, 6, 11, 20, 1),
	(450, 15, 6, 12, 20, 1),

	-- T Level in Management and Administration
	(451, 16, 1, 10, 17, 1),
	(452, 16, 1, 11, 18, 1),
	(453, 16, 1, 12, 18, 1),
	(454, 16, 2, 10, 18, 1),
	(455, 16, 2, 11, 18, 1),
	(456, 16, 2, 12, 19, 1),
	(457, 16, 3, 10, 18, 1),
	(458, 16, 3, 11, 19, 1),
	(459, 16, 3, 12, 19, 1),
	(460, 16, 4, 10, 18, 1),
	(461, 16, 4, 11, 19, 1),
	(462, 16, 4, 12, 20, 1),
	(463, 16, 5, 10, 19, 1),
	(464, 16, 5, 11, 19, 1),
	(465, 16, 5, 12, 20, 1),
	(466, 16, 6, 10, 19, 1),
	(467, 16, 6, 11, 20, 1),
	(468, 16, 6, 12, 20, 1),

	-- T Level in Legal Services
	(469, 17, 1, 10, 17, 1),
	(470, 17, 1, 11, 18, 1),
	(471, 17, 1, 12, 18, 1),
	(472, 17, 2, 10, 18, 1),
	(473, 17, 2, 11, 18, 1),
	(474, 17, 2, 12, 19, 1),
	(475, 17, 3, 10, 18, 1),
	(476, 17, 3, 11, 19, 1),
	(477, 17, 3, 12, 19, 1),
	(478, 17, 4, 10, 18, 1),
	(479, 17, 4, 11, 19, 1),
	(480, 17, 4, 12, 20, 1),
	(481, 17, 5, 10, 19, 1),
	(482, 17, 5, 11, 19, 1),
	(483, 17, 5, 12, 20, 1),
	(484, 17, 6, 10, 19, 1),
	(485, 17, 6, 11, 20, 1),
	(486, 17, 6, 12, 20, 1),

	-- T Level in Agriculture, Land Management and Production
	(487, 18, 1, 10, 17, 1),
	(488, 18, 1, 11, 18, 1),
	(489, 18, 1, 12, 18, 1),
	(490, 18, 2, 10, 18, 1),
	(491, 18, 2, 11, 18, 1),
	(492, 18, 2, 12, 19, 1),
	(493, 18, 3, 10, 18, 1),
	(494, 18, 3, 11, 19, 1),
	(495, 18, 3, 12, 19, 1),
	(496, 18, 4, 10, 18, 1),
	(497, 18, 4, 11, 19, 1),
	(498, 18, 4, 12, 20, 1),
	(499, 18, 5, 10, 19, 1),
	(500, 18, 5, 11, 19, 1),
	(501, 18, 5, 12, 20, 1),
	(502, 18, 6, 10, 19, 1),
	(503, 18, 6, 11, 20, 1),
	(504, 18, 6, 12, 20, 1)

	-- Table 2 End
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
;

SET IDENTITY_INSERT [dbo].[OverallGradeLookup] OFF
