USE BookCatalogDb;
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'admin@bookcatalog.io')
BEGIN
    INSERT INTO Users (Username, Email, PasswordHash)
    VALUES (N'admin', N'admin@bookcatalog.io', N'$2a$12$tHvnUY2jTSJJVQ3w.OnVYOjctvtJXB4SxUexNO3DuMiOQSfirmzWC');
END
GO

IF NOT EXISTS (SELECT 1 FROM Books WHERE ISBN = N'9780441172719')
BEGIN
    INSERT INTO Books (Title, Author, ISBN, PublishedYear) VALUES
    (N'Dune', N'Frank Herbert', N'9780441172719', 1965),
    (N'Beloved', N'Toni Morrison', N'9781400033416', 1987),
    (N'The Pragmatic Programmer', N'David Thomas and Andrew Hunt', N'9780201616224', 1999),
    (N'Circe', N'Madeline Miller', N'9780316556347', 2018),
    (N'The Left Hand of Darkness', N'Ursula K. Le Guin', N'9780441478125', 1969);
END
GO
