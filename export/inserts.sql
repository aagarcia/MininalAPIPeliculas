SET IDENTITY_INSERT Generos ON;
INSERT INTO MinimalPeliculasAPI.dbo.Generos (Id,Nombre) VALUES
	 (1,N'Acción'),
	 (2,N'Drama'),
	 (3,N'Comedia'),
	 (4,N'Biografía'),
	 (9,N'Ciencia Ficción'),
	 (10,N'Cine de guerra'),
	 (11,N'Satírica'),
	 (12,N'Aventura'),
	 (13,N'Superhéroes'),
	 (14,N'Suspenso');
INSERT INTO MinimalPeliculasAPI.dbo.Generos (Id,Nombre) VALUES
	 (15,N'Policial');
SET IDENTITY_INSERT Generos OFF;
SET IDENTITY_INSERT Actores ON;
INSERT INTO MinimalPeliculasAPI.dbo.Actores (Id,Nombre,FechaNacimiento,Foto) VALUES
	 (1,N'Samuel L. Jackson','1948-12-21 00:00:00.0000000',N'https://localhost:7220/actores/656031dd-02af-49d9-a9dc-6ef48bf539ec.jpg'),
	 (2,N'Tom Holland','1996-06-01 00:00:00.0000000',N'https://localhost:7220/actores/89ca06b5-0e4a-4e41-97cd-f4fbb7aaa16d.jpg'),
	 (3,N'Mark Ruffalo','1967-11-22 00:00:00.0000000',N'https://localhost:7220/actores/bb63d7b3-b9a3-4a74-9167-8354ace31e14.jpg'),
	 (4,N'Robert Downey Jr','1965-04-04 00:00:00.0000000',N'https://localhost:7220/actores/299d4cb3-f0f7-496e-b129-be2af1cbc7d6.jpg'),
	 (5,N'Scarlett Johansson','1984-11-22 00:00:00.0000000',N'https://localhost:7220/actores/e2de0a0f-63ea-44a8-9499-7152a8b2b1aa.jpg'),
	 (6,N'Will Smith','1968-09-25 00:00:00.0000000',N'https://localhost:7220/actores/e6838f3d-3d77-4384-a2f9-1abaf84a4dc2.jpg'),
	 (7,N'Will Ferrell','1967-07-16 00:00:00.0000000',N'https://localhost:7220/actores/055b2bef-f01c-4c6d-9a41-922f0472c585.jpg');
SET IDENTITY_INSERT Actores OFF;
SET IDENTITY_INSERT Peliculas ON;
INSERT INTO MinimalPeliculasAPI.dbo.Peliculas (Id,Titulo,EnCines,FechaLanzamiento,Poster) VALUES
	 (1,N'JoJo Rabbit',0,'2019-10-18 00:00:00.0000000',N'https://localhost:7220/peliculas/df283262-c978-4cf0-906b-8514314cf616.jpg'),
	 (2,N'Black Widow',1,'2021-07-09 00:00:00.0000000',N'https://localhost:7220/peliculas/236fc742-5749-42cd-98f2-48e0b31aa771.jpg'),
	 (3,N'Avengers: Endgame',1,'2019-04-26 00:00:00.0000000',N'https://localhost:7220/peliculas/ddd58cbb-cd0d-4293-89f5-545145a61faa.jpg'),
	 (4,N'Bad Boys 1',1,'1995-04-07 00:00:00.0000000',N'https://localhost:7220/peliculas/4df7bd4b-9ae7-4ffd-b3ed-36e1dbeea871.jpg'),
	 (5,N'Bad Boys 2',0,'2003-07-18 00:00:00.0000000',N'https://localhost:7220/peliculas/90baa875-f221-4a15-8973-82513ee22a02.jpg'),
	 (6,N'Zoolander',0,'2001-09-28 00:00:00.0000000',N'https://localhost:7220/peliculas/05e75a4c-ab8a-467f-821c-62bffea3db39.jpg'),
	 (8,N'Zoolander 2',0,'2016-02-12 00:00:00.0000000',N'https://localhost:7220/peliculas/435b0a12-cb1c-4f83-969d-4a6c2fe373b3.jpg');
SET IDENTITY_INSERT Peliculas OFF;
SET IDENTITY_INSERT Comentarios ON;
INSERT INTO MinimalPeliculasAPI.dbo.Comentarios (Id,Cuerpo,PeliculaId) VALUES
	 (1,N'Buena pelicula Jo Jo',1),
	 (3,N'Scarlet excelente actriz',2),
	 (4,N'Excelente pelicula de Marvel',3);
SET IDENTITY_INSERT Comentarios OFF;
INSERT INTO MinimalPeliculasAPI.dbo.GenerosPeliculas (PeliculaId,GeneroId) VALUES
	 (1,2),
	 (1,3),
	 (1,10),
	 (1,11),
	 (2,1),
	 (2,9),
	 (2,12),
	 (2,13),
	 (3,1),
	 (3,2);
INSERT INTO MinimalPeliculasAPI.dbo.GenerosPeliculas (PeliculaId,GeneroId) VALUES
	 (3,9),
	 (3,12),
	 (3,13),
	 (4,1),
	 (4,3),
	 (4,14),
	 (4,15),
	 (5,1),
	 (5,3),
	 (5,14);
INSERT INTO MinimalPeliculasAPI.dbo.GenerosPeliculas (PeliculaId,GeneroId) VALUES
	 (5,15),
	 (6,3),
	 (6,11),
	 (8,3),
	 (8,11);
INSERT INTO MinimalPeliculasAPI.dbo.ActoresPeliculas (ActorId,PeliculaId,Orden,Personaje) VALUES
	 (1,3,2,N'Nick Fury'),
	 (2,3,3,N'Spider-Man'),
	 (3,3,4,N'Bruce Banner'),
	 (4,3,5,N'Iron Man'),
	 (5,1,1,N'Rosie Betzler'),
	 (5,2,1,N'Viuda Negra'),
	 (5,3,1,N'Natasha Romanoff'),
	 (6,4,1,N'Inspecteur Mike Lowrey'),
	 (6,5,1,N'Inspecteur Mike Lowrey'),
	 (7,6,1,N'Jacobim Mugatu');
INSERT INTO MinimalPeliculasAPI.dbo.ActoresPeliculas (ActorId,PeliculaId,Orden,Personaje) VALUES
	 (7,8,1,N'Jacobim Mugatu');