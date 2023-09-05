﻿/*
Insert initial data for Tl Providers
*/

SET IDENTITY_INSERT [dbo].[TlProvider] ON

MERGE INTO [dbo].[TlProvider] AS Target 
USING (VALUES 
(1, 10000080, N'Access Creative College (Access to Music Ltd.)', N'Access Creative College (Access to Music Ltd.)'),
(2, 10000536, N'Barnsley College', N'Barnsley College'),
(3, 10000612, N'Bedfordshire & Luton Education Business Partnership', N'Bedfordshire & Luton Education Business Partnership'),
(4, 10000721, N'Bishop Burton College', N'Bishop Burton College'),
(5, 10000754, N'Blackpool and The Fylde College', N'Blackpool and The Fylde College'),
(6, 10000878, N'Bridgwater & Taunton College', N'Bridgwater & Taunton College'),
(7, 10001165, N'Cardinal Newman College', N'Cardinal Newman College'),
(8, 10007817, N'Chichester College Group', N'Chichester College Group'),
(9, 10001446, N'Cirencester College', N'Cirencester College'),
(10, 10065146, N'City of Stoke-on-Trent sixth Form College', N'City of Stoke-on-Trent sixth Form College'),
(11, 10033241, N'Cranford Community College', N'Cranford Community College'),
(12, 10001919, N'Derby College', N'Derby College'),
(13, 10007924, N'Dudley College of Technology', N'Dudley College of Technology'),
(14, 10002065, N'Durham Sixth Form Centre', N'Durham Sixth Form Centre'),
(15, 10002923, N'East Sussex College Group', N'East Sussex College Group'),
(16, 10002370, N'Exeter College', N'Exeter College'),
(17, 10007928, N'South Hampshire College Group (SHCG)', N'South Hampshire College Group (SHCG)'),
(18, 10002412, N'Farnborough College of Technology', N'Farnborough College of Technology'),
(19, 10002638, N'Gateshead College', N'Gateshead College'),
(20, 10007938, N'TEC Partnership', N'TEC Partnership'),
(21, 10005979, N'Havant and South Downs College', N'Havant and South Downs College'),
(22, 10007193, N'HCUC (Harrow College and Uxbridge College)', N'HCUC (Harrow College and Uxbridge College)'),
(23, 10003732, N'La Retraite RC Girls School', N'La Retraite RC Girls School'),
(24, 10033245, N'Lordswood Girls'' School & Sixth Form Centre', N'Lordswood Girls'' School & Sixth Form Centre'),
(25, 10004552, N'Nelson and Colne College', N'Nelson and Colne College'),
(26, 10004576, N'New College Durham', N'New College Durham'),
(27, 10004772, N'City College Norwich', N'City College Norwich'),
(28, 10004785, N'Notre Dame Catholic Sixth Form College', N'Notre Dame Catholic Sixth Form College'),
(29, 10065145, N'Oldham Sixth Form College', N'Oldham Sixth Form College'),
(30, 10038662, N'Painsley Catholic College', N'Painsley Catholic College'),
(31, 10005072, N'Peter Symonds College', N'Peter Symonds College'),
(32, 10063846, N'Priestley College', N'Priestley College'),
(33, 10005575, N'Runshaw College', N'Runshaw College'),
(34, 10057649, N'Salesian School', N'Salesian School'),
(35, 10017433, N'Sandwell Academy', N'Sandwell Academy'),
(36, 10005687, N'Scarborough Sixth Form College', N'Scarborough Sixth Form College'),
(37, 10005810, N'Shipley College', N'Shipley College'),
(38, 10036413, N'St Thomas More Catholic School Blaydon', N'St Thomas More Catholic School Blaydon'),
(39, 10006378, N'Strode College', N'Strode College'),
(40, 10006398, N'Suffolk New College', N'Suffolk New College'),
(41, 10001550, N'The College of Richard Collyer in Horsham', N'The College of Richard Collyer in Horsham'),
(42, 10047247, N'The Leigh UTC', N'The Leigh UTC'),
(43, 10057638, N'Thorpe St Andrew School and Sixth Form', N'Thorpe St Andrew School and Sixth Form'),
(44, 10007063, N'Truro and Penwith College', N'Truro and Penwith College'),
(45, 10000712, N'University College Birmingham', N'University College Birmingham'),
(46, 10007190, N'Ursuline High School', N'Ursuline High School'),
(47, 10007315, N'Walsall College', N'Walsall College'),
(48, 10042313, N'Walsall Studio School', N'Walsall Studio School'),
(49, 10007459, N'Weston College', N'Weston College'),
(50, 10007709, N'York College', N'York College'),
(51, 10000055, N'Abingdon and Witney College', N'Abingdon and Witney College'),
(52, 10004927, N'Activate Learning', N'Activate Learning'),
(53, 10000528, N'Barking & Dagenham College', N'Barking & Dagenham College'),
(54, 10001465, N'Bath College', N'Bath College'),
(55, 10000610, N'Bedford College Group', N'Bedford College Group'),
(56, 10000670, N'Bexhill College', N'Bexhill College'),
(57, 10000794, N'Bolton College', N'Bolton College'),
(58, 10000473, N'Buckinghamshire College Group', N'Buckinghamshire College Group'),
(59, 10001000, N'Burnley College', N'Burnley College'),
(60, 10001005, N'Bury College', N'Bury College'),
(61, 10001093, N'Calderdale College', N'Calderdale College'),
(62, 10001116, N'Cambridge Regional College', N'Cambridge Regional College'),
(63, 10005972, N'Cheshire College South and West', N'Cheshire College South and West'),
(64, 10001475, N'City of Sunderland College', N'City of Sunderland College'),
(65, 10004695, N'DN College Group', N'DN College Group'),
(66, 10067981, N'East Norfolk Multi Academy Trust', N'East Norfolk Multi Academy Trust'),
(67, 10006570, N'EKC Group', N'EKC Group'),
(68, 10002599, N'Furness College', N'Furness College'),
(69, 10002696, N'Gloucestershire College', N'Gloucestershire College'),
(70, 10002852, N'Halesowen College', N'Halesowen College'),
(71, 10002899, N'Harlow College', N'Harlow College'),
(72, 10007977, N'Heart of Worcestershire College', N'Heart of Worcestershire College'),
(73, 10003023, N'Herefordshire, Ludlow and North Shropshire College', N'Herefordshire, Ludlow and North Shropshire College'),
(74, 10003146, N'Hopwood Hall College', N'Hopwood Hall College'),
(75, 10003193, N'Hugh Baird College', N'Hugh Baird College'),
(76, 10003558, N'Kendal College', N'Kendal College'),
(77, 10003753, N'Lakes College', N'Lakes College'),
(78, 10024962, N'Luminate', N'Luminate'),
(79, 10003867, N'Leicester College', N'Leicester College'),
(80, 10004112, N'Loughborough College', N'Loughborough College'),
(81, 10023139, N'LTE Group trading as The Manchester College', N'LTE Group trading as The Manchester College'),
(82, 10004344, N'Middlesbrough College', N'Middlesbrough College'),
(83, 10004340, N'Midkent College', N'Midkent College'),
(84, 10004375, N'Milton Keynes College', N'Milton Keynes College'),
(85, 10064840, N'Mulberry University Technical College', N'Mulberry University Technical College'),
(86, 10006963, N'New City College', N'New City College'),
(87, 10004579, N'New College Swindon', N'New College Swindon'),
(88, 10004603, N'Newcastle and Stafford Colleges Group', N'Newcastle and Stafford Colleges Group'),
(89, 10004607, N'Newham College of Further Education', N'Newham College of Further Education'),
(90, 10004608, N'Newham Sixth Form College', N'Newham Sixth Form College'),
(91, 10004577, N'Nottingham College', N'Nottingham College'),
(92, 10006770, N'Oldham College', N'Oldham College'),
(93, 10004676, N'Petroc', N'Petroc'),
(94, 10005200, N'Preston College', N'Preston College'),
(95, 10065148, N'Reigate College', N'Reigate College'),
(96, 10005669, N'Sandwell College', N'Sandwell College'),
(97, 10005032, N'Salford City College', N'Salford City College'),
(98, 10005946, N'Solihull College &  University  Centre', N'Solihull College &  University  Centre'),
(99, 10005977, N'South Devon College', N'South Devon College'),
(100, 10005981, N'South Essex College', N'South Essex College'),
(101, 10009439, N'Stanmore College', N'Stanmore College'),
(102, 10007916, N'The College of West Anglia', N'The College of West Anglia'),
(103, 10007455, N'The WKCIC Group', N'The WKCIC Group'),
(104, 10005998, N'Trafford College Group', N'Trafford College Group'),
(105, 10001476, N'United Colleges Group', N'United Colleges Group'),
(106, 10007289, N'The Heart Of Yorkshire Education Group', N'The Heart Of Yorkshire Education Group'),
(107, 10007859, N'Warwickshire College Group', N'Warwickshire College Group'),
(108, 10053988, N'Thomas Telford UTC', N'Thomas Telford UTC'),
(109, 10007431, N'West Suffolk College', N'West Suffolk College'),
(110, 10007500, N'Wigan & Leigh College', N'Wigan & Leigh College'),
(111, 10007503, N'Wilberforce College', N'Wilberforce College'),
(112, 10007673, N'Wyke Sixth Form College', N'Wyke Sixth Form College'),
(113, 10007696, N'Yeovil College', N'Yeovil College'),
(114, 10000560, N'Basingstoke College Of Technology', N'Basingstoke College Of Technology'),
(115, 10000948, N'London and South East Colleges Group', N'London and South East Colleges Group'),
(116, 10000952, N'The SMB Group', N'The SMB Group'),
(117, 10001004, N'Burton And South Derbyshire College', N'Burton And South Derbyshire College'),
(118, 10001201, N'Carmel College', N'Carmel College'),
(119, 10001353, N'Chelsmford College', N'Chelsmford College'),
(120, 10001416, N'Christ The King Sixth Form College', N'Christ The King Sixth Form College'),
(121, 10001743, N'Craven College', N'Craven College'),
(122, 10001778, N'Croydon College', N'Croydon College'),
(123, 10001850, N'Darlington College', N'Darlington College'),
(124, 10002073, N'Big Creative Training Ltd', N'Big Creative Training Ltd'),
(125, 10002111, N'East Durham College', N'East Durham College'),
(126, 10002130, N'East Surrey College', N'East Surrey College'),
(127, 10002443, N'Ferndown Upper School', N'Ferndown Upper School'),
(128, 10002743, N'Grantham College', N'Grantham College'),
(129, 10002863, N'Riverside College', N'Riverside College'),
(130, 10003011, N'The Henley College', N'The Henley College'),
(131, 10003189, N'Kirklees College', N'Kirklees College'),
(132, 10003406, N'Isle Of Wight College', N'Isle Of Wight College'),
(133, 10003855, N'Leeds College Of Building', N'Leeds College Of Building'),
(134, 10003899, N'Leyton Sixth Form College', N'Leyton Sixth Form College'),
(135, 10003928, N'Lincoln College', N'Lincoln College'),
(136, 10003955, N'The City Of Liverpool College', N'The City Of Liverpool College'),
(137, 10004116, N'East Coast College', N'East Coast College'),
(138, 10004144, N'Macclesfield College', N'Macclesfield College'),
(139, 10004596, N'Newbury College', N'Newbury College'),
(140, 10004718, N'North Warwickshire And South Leicestershire College', N'North Warwickshire And South Leicestershire College'),
(141, 10004721, N'North Kent College', N'North Kent College'),
(142, 10005128, N'City College Plymouth', N'City College Plymouth'),
(143, 10005404, N'Reaseheath College', N'Reaseheath College'),
(144, 10005967, N'South & City College Birmingham', N'South & City College Birmingham'),
(145, 10006038, N'Southport College', N'Southport College'),
(146, 10006050, N'Sparsholt College', N'Sparsholt College'),
(147, 10006195, N'St John Rigby Rc Sixth Form College', N'St John Rigby Rc Sixth Form College'),
(148, 10006254, N'St Philomena''s Catholic High School For Girls', N'St Philomena''s Catholic High School For Girls'),
(149, 10006341, N'Education Training Collective', N'Education Training Collective'),
(150, 10006494, N'Tameside College', N'Tameside College'),
(151, 10007011, N'Northampton College', N'Northampton College'),
(152, 10007417, N'West Herts College', N'West Herts College'),
(153, 10007527, N'Wiltshire College And University Centre', N'Wiltshire College And University Centre'),
(154, 10007553, N'Wirral Metropolitan College', N'Wirral Metropolitan College'),
(155, 10007851, N'University Of Derby', N'University Of Derby'),
(156, 10007945, N'Portsmouth College', N'Portsmouth College'),
(157, 10008007, N'St Francis Xavier Sixth Form College', N'St Francis Xavier Sixth Form College'),
(158, 10015295, N'Capital City Academy', N'Capital City Academy'),
(159, 10015885, N'Walsall Academy', N'Walsall Academy'),
(160, 10030656, N'The Jcb Academy', N'The Jcb Academy'),
(161, 10032198, N'Garforth Academy', N'Garforth Academy'),
(162, 10032366, N'The Broxbourne School', N'The Broxbourne School'),
(163, 10034999, N'University Academy Holbeach', N'University Academy Holbeach'),
(164, 10043916, N'St Thomas More Catholic Academy', N'St Thomas More Catholic Academy'),
(165, 10037017, N'The Elizabethan Academy', N'The Elizabethan Academy'),
(166, 10037593, N'Heston Community School', N'Heston Community School'),
(167, 10040000, N'Hedingham School And Sixth Form', N'Hedingham School And Sixth Form'),
(168, 10042219, N'Birchwood Community High School', N'Birchwood Community High School'),
(169, 10042223, N'Burntwood School', N'Burntwood School'),
(170, 10043511, N'Cambridge Academy For Science And Technology', N'Cambridge Academy For Science And Technology'),
(171, 10047109, N'Lincoln Utc', N'Lincoln Utc'),
(172, 10047244, N'University Technical College Norfolk', N'University Technical College Norfolk'),
(173, 10053993, N'Somerset Studio School', N'Somerset Studio School'),
(174, 10057006, N'Crewe Engineering And Design Utc', N'Crewe Engineering And Design Utc'),
(175, 10057025, N'Utc South Durham', N'Utc South Durham'),
(176, 10064686, N'Birkenhead Sixth Form College', N'Birkenhead Sixth Form College'),
(177, 10082366, N'Ashton Sixth Form College', N'Ashton Sixth Form College'),
(178, 10085525, N'Gateway Sixth Form College', N'Gateway Sixth Form College'),
(179, 10036143, N'South Gloucestershire And Stroud College', N'South Gloucestershire And Stroud College'),
(180, 10039896, N'Abbeywood Community School', N'Abbeywood Community School'),
(181, 10000101, N'Acland Burghley School', N'Acland Burghley School'),
(182, 10067896, N'Ada Lovelace Church of England High School', N'Ada Lovelace Church of England High School'),
(183, 10057981, N'Ada National College for Digital Skills', N'Ada National College for Digital Skills'),
(184, 10040003, N'Aldersley High School', N'Aldersley High School'),
(185, 10027911, N'All Saints Catholic High School', N'All Saints Catholic High School'),
(186, 10033331, N'Archbishop Holgate''s School', N'Archbishop Holgate''s School'),
(187, 10083729, N'Ark Pioneer Academy', N'Ark Pioneer Academy'),
(188, 10000415, N'Askham Bryan College', N'Askham Bryan College'),
(189, 10037492, N'Aston University Engineering Academy', N'Aston University Engineering Academy'),
(190, 10041653, N'Aylesbury UTC', N'Aylesbury UTC'),
(191, 10000533, N'Barnet and Southgate College', N'Barnet and Southgate College'),
(192, 10054167, N'Barr''s Hill School and Community College', N'Barr''s Hill School and Community College'),
(193, 10000597, N'Beauchamps High School', N'Beauchamps High School'),
(194, 10027529, N'Bede Academy', N'Bede Academy'),
(195, 10046350, N'Big Creative Academy', N'Big Creative Academy'),
(196, 10006442, N'Birmingham Metropolitan College', N'Birmingham Metropolitan College'),
(197, 10000720, N'Bishop Auckland College', N'Bishop Auckland College'),
(198, 10000722, N'Bishop Challoner Catholic College', N'Bishop Challoner Catholic College'),
(199, 10000747, N'Blackburn College', N'Blackburn College'),
(200, 10036163, N'Bluecoat Aspley Academy', N'Bluecoat Aspley Academy'),
(201, 10088739, N'BOA Stage and Screen Production Academy', N'BOA Stage and Screen Production Academy'),
(202, 10032351, N'Bodmin College', N'Bodmin College'),
(203, 10000796, N'Bolton Sixth Form College', N'Bolton Sixth Form College'),
(204, 10014824, N'Bordesley Green Girls'' School and Sixth Form', N'Bordesley Green Girls'' School and Sixth Form'),
(205, 10000812, N'Boston College', N'Boston College'),
(206, 10000840, N'Bradford College', N'Bradford College'),
(207, 10030697, N'Brighton Aldridge Community Academy', N'Brighton Aldridge Community Academy'),
(208, 10000944, N'Brockenhurst College', N'Brockenhurst College'),
(209, 10030223, N'Brompton Academy', N'Brompton Academy'),
(210, 10089864, N'Brook Sixth Form and Academy', N'Brook Sixth Form and Academy'),
(211, 10000950, N'Brooklands College', N'Brooklands College'),
(212, 10001095, N'Calderstones School', N'Calderstones School'),
(213, 10064832, N'Callywith College', N'Callywith College'),
(214, 10034661, N'Canons High School', N'Canons High School'),
(215, 10001148, N'Capel Manor College', N'Capel Manor College'),
(216, 10008145, N'Cardinal Newman Catholic School', N'Cardinal Newman Catholic School'),
(217, 10033761, N'Carshalton High School for Girls', N'Carshalton High School for Girls'),
(218, 10001259, N'Central Training Academy Limited', N'Central Training Academy Limited'),
(219, 10001378, N'Chesterfield College', N'Chesterfield College'),
(220, 10036744, N'Chiswick School', N'Chiswick School'),
(221, 10034170, N'Christopher Whitehead Language College', N'Christopher Whitehead Language College'),
(222, 10001467, N'City of Bristol College', N'City of Bristol College'),
(223, 10007578, N'City of Wolverhampton College', N'City of Wolverhampton College'),
(224, 10028138, N'Clacton Coastal Academy', N'Clacton Coastal Academy'),
(225, 10036670, N'Clacton County High School', N'Clacton County High School'),
(226, 10001535, N'Colchester Institute', N'Colchester Institute'),
(227, 10091127, N'Coloma Convent Girls'' School', N'Coloma Convent Girls'' School'),
(228, 10040290, N'Conyers School', N'Conyers School'),
(229, 10001696, N'Cornwall College', N'Cornwall College'),
(230, 10003010, N'Coventry College', N'Coventry College'),
(231, 10024130, N'Darwen Aldridge Community Academy', N'Darwen Aldridge Community Academy'),
(232, 10042599, N'Darwen Aldridge Enterprise Studio', N'Darwen Aldridge Enterprise Studio'),
(233, 10033037, N'De Aston School', N'De Aston School'),
(234, 10037567, N'De Lisle College Loughborough Leicestershire', N'De Lisle College Loughborough Leicestershire'),
(235, 10066363, N'Derby Moor Spencer Academy', N'Derby Moor Spencer Academy'),
(236, 10001934, N'Derwentside College', N'Derwentside College'),
(237, 10086697, N'Doncaster UTC', N'Doncaster UTC'),
(238, 10066365, N'Dormers Wells High School', N'Dormers Wells High School'),
(239, 10002094, N'Ealing Hammersmith and West London College', N'Ealing Hammersmith and West London College'),
(240, 10066367, N'Edgbarrow School', N'Edgbarrow School'),
(241, 10061451, N'Elizabeth Woodville School', N'Elizabeth Woodville School'),
(242, 10053859, N'Elliott Hudson College', N'Elliott Hudson College'),
(243, 10041658, N'Elstree Screen Arts Academy', N'Elstree Screen Arts Academy'),
(244, 10046227, N'Energy Coast UTC', N'Energy Coast UTC'),
(245, 10046561, N'Engineering UTC Northern Lincolnshire', N'Engineering UTC Northern Lincolnshire'),
(246, 10035797, N'Featherstone High School', N'Featherstone High School'),
(247, 10002570, N'Franklin College', N'Franklin College'),
(248, 10002584, N'Frome Community College', N'Frome Community College'),
(249, 10040656, N'Fulham Cross Academy', N'Fulham Cross Academy'),
(250, 10057007, N'Global Academy', N'Global Academy'),
(251, 10034657, N'Great Marlow School', N'Great Marlow School'),
(252, 10057009, N'Greater Peterborough UTC', N'Greater Peterborough UTC'),
(253, 10039056, N'Grey Court School', N'Grey Court School'),
(254, 10040630, N'Haringey Sixth Form College', N'Haringey Sixth Form College'),
(255, 10046287, N'Harris Academy Battersea', N'Harris Academy Battersea'),
(256, 10034922, N'Harris Academy Beckenham', N'Harris Academy Beckenham'),
(257, 10027542, N'Harris Boys'' Academy East Dulwich', N'Harris Boys'' Academy East Dulwich'),
(258, 10091327, N'Harris Science Academy East London', N'Harris Science Academy East London'),
(259, 10002917, N'Hartlepool College of Further Education', N'Hartlepool College of Further Education'),
(260, 10080810, N'Hartpury College', N'Hartpury College'),
(261, 10034669, N'Hatch End High School', N'Hatch End High School'),
(262, 10002938, N'Haverstock School', N'Haverstock School'),
(263, 10031580, N'Hayesbrook Academy', N'Hayesbrook Academy'),
(264, 10002976, N'Heart of England Training Limited', N'Heart of England Training Limited'),
(265, 10002998, N'Helsby High School', N'Helsby High School'),
(266, 10003035, N'Hertford Regional College', N'Hertford Regional College'),
(267, 10053915, N'Highfields School', N'Highfields School'),
(268, 10087521, N'Hinckley Academy and John Cleveland Sixth Form Centre', N'Hinckley Academy and John Cleveland Sixth Form Centre'),
(269, 10039989, N'Hitchin Boys'' School', N'Hitchin Boys'' School'),
(270, 10015760, N'Holly Lodge High School College of Science', N'Holly Lodge High School College of Science'),
(271, 10003200, N'Hull College', N'Hull College'),
(272, 10005077, N'Inspire Education Group', N'Inspire Education Group'),
(273, 10003427, N'Itchen College', N'Itchen College'),
(274, 10001685, N'James Calvert Spence College', N'James Calvert Spence College'),
(275, 10034617, N'John Colet School', N'John Colet School'),
(276, 10032599, N'John Henry Newman Catholic College', N'John Henry Newman Catholic College'),
(277, 10003491, N'John Leggott College', N'John Leggott College'),
(278, 10003511, N'Joseph Chamberlain Sixth Form College', N'Joseph Chamberlain Sixth Form College'),
(279, 10036183, N'Joseph Leckie Academy', N'Joseph Leckie Academy'),
(280, 10033365, N'Kennet School', N'Kennet School'),
(281, 10081066, N'King Henry School', N'King Henry School'),
(282, 10053917, N'Kingsmead School', N'Kingsmead School'),
(283, 10003676, N'Kingston Maurward College', N'Kingston Maurward College'),
(284, 10003733, N'La Sainte Union Catholic Secondary School', N'La Sainte Union Catholic Secondary School'),
(285, 10003755, N'Lambeth College', N'Lambeth College'),
(286, 10003768, N'Lancaster and Morecambe College', N'Lancaster and Morecambe College'),
(287, 10044606, N'Landau Forte Academy Tamworth Sixth Form', N'Landau Forte Academy Tamworth Sixth Form'),
(288, 10003923, N'Lilian Baylis Technology School', N'Lilian Baylis Technology School'),
(289, 10041690, N'Liverpool Life Sciences UTC', N'Liverpool Life Sciences UTC'),
(290, 10057010, N'London Design and Engineering UTC', N'London Design and Engineering UTC'),
(291, 10004088, N'Long Road Sixth Form College', N'Long Road Sixth Form College'),
(292, 10034510, N'Longdean School', N'Longdean School'),
(293, 10024300, N'Longfield Academy', N'Longfield Academy'),
(294, 10064664, N'Longley Park Sixth Form Academy', N'Longley Park Sixth Form Academy'),
(295, 10034939, N'Longsands Academy', N'Longsands Academy'),
(296, 10004147, N'Macmillan Academy', N'Macmillan Academy'),
(297, 10004201, N'Maria Fidelis Catholic School FCJ', N'Maria Fidelis Catholic School FCJ'),
(298, 10013987, N'Mary Hare School', N'Mary Hare School'),
(299, 10034109, N'Mascalls Academy', N'Mascalls Academy'),
(300, 10004236, N'Matravers School', N'Matravers School'),
(301, 10087691, N'Meadow Park School', N'Meadow Park School'),
(302, 10004440, N'M I T Skills Limited', N'M I T Skills Limited'),
(303, 10004432, N'Morley College London', N'Morley College London'),
(304, 10004442, N'Moulton College', N'Moulton College'),
(305, 10004478, N'Myerscough College', N'Myerscough College'),
(306, 10027549, N'NCEA Duke''s Secondary School', N'NCEA Duke''s Secondary School'),
(307, 10004599, N'NCG', N'NCG'),
(308, 10036555, N'Netherthorpe School', N'Netherthorpe School'),
(309, 10083760, N'New College Bradford', N'New College Bradford'),
(310, 10063721, N'New College Pontefract', N'New College Pontefract'),
(311, 10068133, N'North East Futures UTC', N'North East Futures UTC'),
(312, 10004686, N'North East Surrey College of Technology', N'North East Surrey College of Technology'),
(313, 10004690, N'North Hertfordshire College', N'North Hertfordshire College'),
(314, 10004692, N'North Lancs Training Group Limited', N'North Lancs Training Group Limited'),
(315, 10004755, N'Northfleet Technology College', N'Northfleet Technology College'),
(316, 10004758, N'Northolt High School', N'Northolt High School'),
(317, 10025622, N'Nottingham University Samworth Academy', N'Nottingham University Samworth Academy'),
(318, 10004835, N'Oaklands College', N'Oaklands College'),
(319, 10084304, N'Onslow St Audrey''s School', N'Onslow St Audrey''s School'),
(320, 10031058, N'Ormiston Victory Academy', N'Ormiston Victory Academy'),
(321, 10032973, N'Ossett Academy and Sixth Form College', N'Ossett Academy and Sixth Form College'),
(322, 10027986, N'Outwood Academy Adwick', N'Outwood Academy Adwick'),
(323, 10004992, N'Parliament Hill School', N'Parliament Hill School'),
(324, 10005074, N'Peterborough City Council', N'Peterborough City Council'),
(325, 10005124, N'Plumpton College', N'Plumpton College'),
(326, 10005126, N'Plymouth City Council', N'Plymouth City Council'),
(327, 10005145, N'Poole High School', N'Poole High School'),
(328, 10005172, N'Poultec Training Limited', N'Poultec Training Limited'),
(329, 10043454, N'President Kennedy School Academy', N'President Kennedy School Academy'),
(330, 10032987, N'Prince Henry''s High School', N'Prince Henry''s High School'),
(331, 10065941, N'Prior Pursglove and Stockton Sixth Form College', N'Prior Pursglove and Stockton Sixth Form College'),
(332, 10005426, N'Reflections Training Academy', N'Reflections Training Academy'),
(333, 10005970, N'Regent High School', N'Regent High School'),
(334, 10089242, N'Richard Huish College', N'Richard Huish College'),
(335, 10065834, N'Richard Taunton Sixth Form College', N'Richard Taunton Sixth Form College'),
(336, 10061803, N'Rivington and Blackrod High School', N'Rivington and Blackrod High School'),
(337, 10005534, N'RNN Group', N'RNN Group'),
(338, 10037774, N'Rodillian Academy', N'Rodillian Academy'),
(339, 10064847, N'Ron Dearing UTC', N'Ron Dearing UTC'),
(340, 10062245, N'Royal Greenwich Trust School', N'Royal Greenwich Trust School'),
(341, 10005558, N'Royal National College for the Blind', N'Royal National College for the Blind'),
(342, 10006005, N'S.W. Durham Training Limited', N'S.W. Durham Training Limited'),
(343, 10036715, N'Sacred Heart High School', N'Sacred Heart High School'),
(344, 10035681, N'Saint George''s Church of England School', N'Saint George''s Church of England School'),
(345, 10035180, N'Saint John Bosco College', N'Saint John Bosco College'),
(346, 10068106, N'Saracens High School', N'Saracens High School'),
(347, 10064844, N'SGS Berkeley Green UTC', N'SGS Berkeley Green UTC'),
(348, 10035662, N'Sheringham High School', N'Sheringham High School'),
(349, 10033334, N'Shire Oak Academy', N'Shire Oak Academy'),
(350, 10039420, N'Shooters Hill Sixth Form College', N'Shooters Hill Sixth Form College'),
(351, 10028147, N'Shoreham Academy', N'Shoreham Academy'),
(352, 10005822, N'Shrewsbury Colleges Group', N'Shrewsbury Colleges Group'),
(353, 10041717, N'Silverstone UTC', N'Silverstone UTC'),
(354, 10005859, N'Sir George Monoux College', N'Sir George Monoux College'),
(355, 10064663, N'Solihull Sixth Form College', N'Solihull Sixth Form College'),
(356, 10042505, N'Somerset Skills and Learning CIC', N'Somerset Skills and Learning CIC'),
(357, 10057011, N'South Bank UTC', N'South Bank UTC'),
(358, 10053985, N'South Devon UTC', N'South Devon UTC'),
(359, 10023526, N'South Staffordshire College', N'South Staffordshire College'),
(360, 10003674, N'South Thames Colleges Group', N'South Thames Colleges Group'),
(361, 10037105, N'Southmoor Academy', N'Southmoor Academy'),
(362, 10035080, N'St Albans Girls'' School', N'St Albans Girls'' School'),
(363, 10006135, N'St Charles Catholic Sixth Form College', N'St Charles Catholic Sixth Form College'),
(364, 10029212, N'St George''s Academy', N'St George''s Academy'),
(365, 10006174, N'St Helen''s College', N'St Helen''s College'),
(366, 10006188, N'St John Bosco Arts College', N'St John Bosco Arts College'),
(367, 10042275, N'St Joseph''s Catholic Academy', N'St Joseph''s Catholic Academy'),
(368, 10061777, N'St Mary''s Catholic College', N'St Mary''s Catholic College'),
(369, 10063694, N'St Mary''s College', N'St Mary''s College'),
(370, 10083667, N'St Robert of Newminster Catholic School and SFC', N'St Robert of Newminster Catholic School and SFC'),
(371, 10065835, N'St Vincent College', N'St Vincent College'),
(372, 10047139, N'Stationers'' Crown Woods Academy', N'Stationers'' Crown Woods Academy'),
(373, 10006349, N'Stoke on Trent College', N'Stoke on Trent College'),
(374, 10065476, N'Stoke Park School', N'Stoke Park School'),
(375, 10083768, N'Stone Lodge School', N'Stone Lodge School'),
(376, 10006456, N'Swanshurst School', N'Swanshurst School'),
(377, 10006549, N'Telford College', N'Telford College'),
(378, 10081697, N'Temple Moor High School', N'Temple Moor High School'),
(379, 10000756, N'The Blackpool Sixth Form College', N'The Blackpool Sixth Form College'),
(380, 10006602, N'The Blandford School', N'The Blandford School'),
(381, 10000820, N'The Bournemouth and Poole College', N'The Bournemouth and Poole College'),
(382, 10006813, N'The Brooke House Sixth Form College', N'The Brooke House Sixth Form College'),
(383, 10031579, N'The Canterbury Academy', N'The Canterbury Academy'),
(384, 10034819, N'The Coleshill School', N'The Coleshill School'),
(385, 10066521, N'The Halley Academy', N'The Halley Academy'),
(386, 10034985, N'The Hundred of Hoo Academy', N'The Hundred of Hoo Academy'),
(387, 10087514, N'The King''s CofE Academy', N'The King''s CofE Academy'),
(388, 10021033, N'The Leigh Academy', N'The Leigh Academy'),
(389, 10006765, N'The Nobel School', N'The Nobel School'),
(390, 10088876, N'The North School', N'The North School'),
(391, 10004797, N'The Nottingham Trent University', N'The Nottingham Trent University'),
(392, 10030873, N'The Phoenix Collegiate', N'The Phoenix Collegiate'),
(393, 10024303, N'The Priory City of Lincoln Academy', N'The Priory City of Lincoln Academy'),
(394, 10031140, N'The Priory Ruskin Academy', N'The Priory Ruskin Academy'),
(395, 10024145, N'The Priory Witham Academy', N'The Priory Witham Academy'),
(396, 10033410, N'The Ravensbourne School', N'The Ravensbourne School'),
(397, 10005788, N'The Sheffield College', N'The Sheffield College'),
(398, 10036067, N'The Sittingbourne School', N'The Sittingbourne School'),
(399, 10035564, N'The UCL Academy', N'The UCL Academy'),
(400, 10062638, N'Thomas Alleyne''s High School', N'Thomas Alleyne''s High School'),
(401, 10065942, N'Thomas Rotherham College', N'Thomas Rotherham College'),
(402, 10021755, N'Total People', N'Total People'),
(403, 10054172, N'Trinity Catholic College', N'Trinity Catholic College'),
(404, 10033367, N'Twynham School', N'Twynham School'),
(405, 10005999, N'Tyne Coast College', N'Tyne Coast College'),
(406, 10007126, N'Ullswater Community College', N'Ullswater Community College'),
(407, 10057012, N'University Technical College Leeds', N'University Technical College Leeds'),
(408, 10005736, N'USP College', N'USP College'),
(409, 10085102, N'UTC Derby Pride Park', N'UTC Derby Pride Park'),
(410, 10081518, N'UTC Plymouth', N'UTC Plymouth'),
(411, 10064710, N'UTC Portsmouth', N'UTC Portsmouth'),
(412, 10041720, N'UTC Sheffield City Centre', N'UTC Sheffield City Centre'),
(413, 10057013, N'UTC Sheffield Olympic Legacy Park', N'UTC Sheffield Olympic Legacy Park'),
(414, 10057015, N'UTC Warrington', N'UTC Warrington'),
(415, 10007212, N'Varndean College', N'Varndean College'),
(416, 10026108, N'Virtual Alliance Limited', N'Virtual Alliance Limited'),
(417, 10007321, N'Waltham Forest College', N'Waltham Forest College'),
(418, 10034929, N'Walton High', N'Walton High'),
(419, 10007339, N'Warrington and Vale Royal College', N'Warrington and Vale Royal College'),
(420, 10035973, N'Weatherhead High School', N'Weatherhead High School'),
(421, 10007427, N'West Nottinghamshire College', N'West Nottinghamshire College'),
(422, 10007434, N'West Thames College', N'West Thames College'),
(423, 10031371, N'Westlands School', N'Westlands School'),
(424, 10007469, N'Weymouth College', N'Weymouth College'),
(425, 10047145, N'Whalley Range 11-18 High School', N'Whalley Range 11-18 High School'),
(426, 10007511, N'William Ellis School', N'William Ellis School'),
(427, 10006864, N'William Morris Sixth Form', N'William Morris Sixth Form'),
(428, 10031094, N'Wilmington Academy', N'Wilmington Academy'),
(429, 10007531, N'Wimbledon College', N'Wimbledon College'),
(430, 10002107, N'Windsor Forest Colleges Group', N'Windsor Forest Colleges Group'),
(431, 10007546, N'Winstanley College', N'Winstanley College'),
(432, 10046653, N'WMG Academy for Young Engineers', N'WMG Academy for Young Engineers'),
(433, 10057017, N'WMG Academy for Young Engineers (Solihull)', N'WMG Academy for Young Engineers (Solihull)'),
(434, 10065366, N'Worcester Sixth Form College', N'Worcester Sixth Form College'),
(435, 10035033, N'Writhlington School', N'Writhlington School'),
(436, 10007657, N'Writtle University College', N'Writtle University College'),
(437, 10034175, N'Yavneh College', N'Yavneh College')
)
AS Source ([Id], [UkPrn], [Name], [DisplayName]) 
ON Target.[Id] = Source.[Id] 
-- Update from Source when Id is Matched
WHEN MATCHED 
	 AND ((Target.[UkPrn] <> Source.[UkPrn])
	   OR (Target.[Name] <> Source.[Name] COLLATE Latin1_General_CS_AS)
	   OR (Target.[DisplayName] <> Source.[DisplayName] COLLATE Latin1_General_CS_AS))
THEN 
UPDATE SET 
	[UkPrn] = Source.[UkPrn],
	[Name] = Source.[Name],
	[DisplayName] = Source.[DisplayName],
	[ModifiedOn] = GETDATE(),
	[ModifiedBy] = 'System'
WHEN NOT MATCHED BY TARGET THEN 
	INSERT ([Id], [UkPrn], [Name], [DisplayName], [CreatedBy]) 
	VALUES ([Id], [UkPrn], [Name], [DisplayName], 'System') 
WHEN NOT MATCHED BY SOURCE THEN 
DELETE;

SET IDENTITY_INSERT [dbo].[TlProvider] OFF
