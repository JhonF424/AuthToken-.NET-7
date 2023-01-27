CREATE DATABASE jwtdbcurso;

USE jwtdbcurso;

CREATE TABLE Users(
	UserID INT PRIMARY KEY IDENTITY,
	Username VARCHAR(20),
	Pass VARCHAR(20)
);

INSERT INTO Users(Username, Pass) VALUES ('Admin', '12345678');

SELECT * FROM USERS;

CREATE TABLE HistoryRefreshToken(
	IdHistoryToken INT PRIMARY KEY IDENTITY,
	UserID INT REFERENCES Users(UserID),
	Token VARCHAR(500),
	RefreshToken VARCHAR(200),
	CreationDate DATETIME,
	ExpirationDate DATETIME,
	isActive AS (iif (ExpirationDate < getDate(), convert(bit, 0), convert(bit, 1)))
);