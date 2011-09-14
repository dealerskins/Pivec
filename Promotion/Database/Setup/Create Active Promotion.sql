DECLARE @Name VARCHAR(50)

-- Please assign the name of the active promotion

SET @Name = 'aaaaa'

INSERT INTO dbo.Promotions ( Id, Name, IsActive )
VALUES ( NewId(), @Name, 1 )