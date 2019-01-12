 
CREATE TABLE [dbo].[Palindromes](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PalindromeWord] [nvarchar](200) NULL
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Palindromes] ON 

INSERT [dbo].[Palindromes] ([Id], [PalindromeWord]) VALUES (1, N'Was it a cat I saw')
INSERT [dbo].[Palindromes] ([Id], [PalindromeWord]) VALUES (2, N'Don''t nod')
INSERT [dbo].[Palindromes] ([Id], [PalindromeWord]) VALUES (3, N'Radar')
INSERT [dbo].[Palindromes] ([Id], [PalindromeWord]) VALUES (4, N'No lemon, no melon')
SET IDENTITY_INSERT [dbo].[Palindromes] OFF
