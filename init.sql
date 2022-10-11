CREATE TABLE IF NOT EXISTS usuarios_config (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	usarEmail BOOL default false,
	usarNumeroCelular BOOL default false
);


CREATE TABLE IF NOT EXISTS usuarios (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	email text NOT NULL,
	senha text NOT NULL,
	nome text NULL,
    numero_celular text NULL,
	usuario_config_id INT NOT NULL
);


ALTER TABLE usuarios
        ADD FOREIGN KEY (id) REFERENCES usuarios_config (id)
               DEFERRABLE INITIALLY DEFERRED on delete cascade on update cascade;
 
ALTER TABLE usuarios_config 
        ADD FOREIGN KEY (id) REFERENCES usuarios (id)
                DEFERRABLE INITIALLY DEFERRED;
