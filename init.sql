CREATE TABLE IF NOT EXISTS usuarios (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	email text NOT NULL UNIQUE,
	senha text NOT NULL,
	nome text NULL,
	ultimo_login timestamp NULL,
    numero_celular text NULL,
	usuario_config_id INT NOT NULL,
	role int default 0

);

CREATE TABLE IF NOT EXISTS usuarios_config (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	usaremail BOOL default false,
	usarnumerocelular BOOL default false
);

CREATE TABLE IF NOT EXISTS api_keys (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	apikey text NOT NULL,
	scopes text NOT NULL,
	usuarioid INT NOT NULL,
	creatAt timestamp NULL,
	updateAt timestamp NULL
);

/*ALTER TABLE usuarios_config
ADD FOREIGN KEY (id) REFERENCES usuarios (id)
on delete cascade on update cascade
DEFERRABLE INITIALLY DEFERRED;*/

ALTER TABLE usuarios
ADD FOREIGN KEY (id) REFERENCES usuarios_config (id)
on delete cascade on update cascade
DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE api_keys
ADD FOREIGN KEY (usuarioid) REFERENCES usuarios (id)
on delete cascade on update cascade
DEFERRABLE INITIALLY DEFERRED;
