CREATE TABLE IF NOT EXISTS usuarios (
    usuarioid INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	email text NOT NULL,
	senha text NOT NULL,
	nome text NULL
);