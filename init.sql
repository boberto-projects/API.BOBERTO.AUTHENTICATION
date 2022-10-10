-- To jogando fora o entity framework e o aspnetuers fazendo isso.
-- To jogando fora o entity framework e o aspnetuers fazendo isso.

CREATE TABLE IF NOT EXISTS usuarios (
    usuario_id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	email text NOT NULL,
	senha text NOT NULL,
	nome text NULL,
    numero_celular text NULL,
	duplo_auth BOOL default false
);

CREATE TABLE IF NOT EXISTS usuariosauth (
    usuariosauth_id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	usarEmail BOOL default false,
	usarNumeroCelular BOOL default false,
	usuario_id INT NOT NULL,
	CONSTRAINT fk_usuarios FOREIGN KEY(usuario_id) REFERENCES usuarios(usuario_id)
);




