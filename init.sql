CREATE TABLE IF NOT EXISTS users (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	email text NOT NULL UNIQUE,
	password text NOT NULL,
	name text NULL,
	last_login timestamp NULL,
    phone_number text NULL,
	users_config_id INT NOT NULL,
	role int default 0

);

CREATE TABLE IF NOT EXISTS users_config (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	enabled_email BOOL default false,
	enabled_phone_number BOOL default false
);

CREATE TABLE IF NOT EXISTS api_keys (
    id INT PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
	apikey text NOT NULL,
	scopes text NOT NULL,
	userid INT NOT NULL,
	creatAt timestamp NULL,
	updateAt timestamp NULL
);

/*ALTER TABLE usuarios_config
ADD FOREIGN KEY (id) REFERENCES usuarios (id)
on delete cascade on update cascade
DEFERRABLE INITIALLY DEFERRED;*/

ALTER TABLE users
ADD FOREIGN KEY (id) REFERENCES users_config (id)
on delete cascade on update cascade
DEFERRABLE INITIALLY DEFERRED;

ALTER TABLE api_keys
ADD FOREIGN KEY (userid) REFERENCES users (id)
on delete cascade on update cascade
DEFERRABLE INITIALLY DEFERRED;