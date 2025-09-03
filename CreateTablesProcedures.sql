
-- Tabla de usuarios
CREATE TABLE usuarios (
    usuario_id SERIAL PRIMARY KEY,
    usuario VARCHAR(100) NOT NULL,
	contrasenia VARCHAR(50) NOT NULL,
    correo VARCHAR(150) UNIQUE NOT NULL,
    fecha_registro TIMESTAMP DEFAULT NOW()
);

-- Tabla de deudas
CREATE TABLE deudas (
    deuda_id SERIAL PRIMARY KEY,
    deudor_id INT NOT NULL REFERENCES usuarios(usuario_id),
    acreedor_id INT NOT NULL REFERENCES usuarios(usuario_id),
    monto NUMERIC(12,2) NOT NULL CHECK (monto > 0),
    saldo NUMERIC(12,2) NOT NULL CHECK (saldo >= 0),
    estado VARCHAR(20) DEFAULT 'PENDIENTE' CHECK (estado IN ('PENDIENTE', 'PAGADA')),
    fecha_creacion TIMESTAMP DEFAULT NOW()
);

-- Tabla de pagos
CREATE TABLE pagos (
    pago_id SERIAL PRIMARY KEY,
    deuda_id INT NOT NULL REFERENCES deudas(deuda_id),
    monto NUMERIC(12,2) NOT NULL CHECK (monto > 0),
    fecha_pago TIMESTAMP DEFAULT NOW()
);




------------------------ (Funciones) -------------------------------

-------- (Usuarios) ----------
-- Funcion para crear usuario
CREATE OR REPLACE FUNCTION fn_usuario_crear(
    p_usuario VARCHAR,
    p_correo VARCHAR,
	p_contrasenia VARCHAR
) RETURNS VOID AS
$$
BEGIN
    INSERT INTO usuarios (usuario, correo, contrasenia)
    VALUES (p_usuario, p_correo, p_contrasenia);
END;
$$ LANGUAGE plpgsql;

-- Funcion para editar usuario
CREATE OR REPLACE FUNCTION fn_usuario_editar(
    p_usuario_id INT,
    p_usuario VARCHAR,
    p_correo VARCHAR,
	p_contrasenia VARCHAR
) RETURNS VOID AS
$$
BEGIN
    UPDATE usuarios
    SET usuario = p_usuario,
        correo = p_correo,
		contrasenia = p_contrasenia
    WHERE usuario_id = p_usuario_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'El usuario con ID % no existe', p_usuario_id;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Funcion para eliminar usuario
CREATE OR REPLACE FUNCTION fn_usuario_eliminar(
    p_usuario_id INT
) RETURNS VOID AS
$$
DECLARE
    v_existe INT;
BEGIN
    -- Validar si tiene deudas relacionadas
    SELECT COUNT(*) INTO v_existe
    FROM deudas
    WHERE deudor_id = p_usuario_id OR acreedor_id = p_usuario_id;

    IF v_existe > 0 THEN
        RAISE EXCEPTION 'No se puede eliminar el usuario porque tiene deudas registradas';
    END IF;

    DELETE FROM usuarios WHERE usuario_id = p_usuario_id;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'El usuario con ID % no existe', p_usuario_id;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Funcion para consultar usuario
CREATE OR REPLACE FUNCTION fn_usuario_consultar(
    p_usuario_id INT
) RETURNS TABLE (
    usuario_id INT,
    usuario VARCHAR,
    correo VARCHAR,
	contrasenia VARCHAR,
    fecha_registro TIMESTAMP
) AS
$$
BEGIN
    RETURN QUERY
    SELECT u.usuario_id, u.usuario, u.correo, u.contrasenia, u.fecha_registro
    FROM usuarios u
    WHERE u.usuario_id = p_usuario_id;
END;
$$ LANGUAGE plpgsql;


-- Funcion para listar usuarios
CREATE OR REPLACE FUNCTION fn_usuario_listar()
RETURNS TABLE (
    usuario_id INT,
    usuario VARCHAR,
    correo VARCHAR,
	contrasenia VARCHAR,
    fecha_registro TIMESTAMP
) AS
$$
BEGIN
    RETURN QUERY
    SELECT u.usuario_id, u.usuario, u.correo, u.contrasenia, u.fecha_registro
    FROM usuarios u
    ORDER BY u.nombre;
END;
$$ LANGUAGE plpgsql;


-------- (Deudas) ----------
-- Funcion para la creacion de deudas
CREATE OR REPLACE FUNCTION fn_deuda_crear(
    p_deudor_id INT,
    p_acreedor_id INT,
    p_monto NUMERIC
) RETURNS VOID AS 
$$
BEGIN
    INSERT INTO deudas (deudor_id, acreedor_id, monto, saldo)
    VALUES (p_deudor_id, p_acreedor_id, p_monto, p_monto);
END;
$$ 
LANGUAGE plpgsql;

-- Funcion para editar una deuda
CREATE OR REPLACE FUNCTION fn_deuda_editar(
    p_deuda_id INT,
    p_monto NUMERIC
) RETURNS VOID AS 
$$
DECLARE
    v_estado VARCHAR(20);
BEGIN
    -- Obtener estado actual
    SELECT estado INTO v_estado 
    FROM deudas 
    WHERE deuda_id = p_deuda_id;
	-- Validar si aun es posible actualizarla
    IF v_estado IS NULL THEN
        RAISE EXCEPTION 'La deuda no existe';
    ELSIF v_estado = 'PAGADA' THEN
        RAISE EXCEPTION 'No se puede editar una deuda ya pagada';
    END IF;
    -- Actualizar monto y saldo
    UPDATE deudas
    SET monto = p_monto,
        saldo = p_monto
    WHERE deuda_id = p_deuda_id;
END;
$$ LANGUAGE plpgsql;

-- Funcion para eliminar una deuda
CREATE OR REPLACE FUNCTION fn_deuda_eliminar(
    p_deuda_id INT
) RETURNS VOID AS 
$$
DECLARE
    v_monto NUMERIC;
    v_saldo NUMERIC;
BEGIN
    SELECT monto, saldo INTO v_monto, v_saldo
    FROM deudas
    WHERE deuda_id = p_deuda_id;
	-- Validar si se puede eliminar la deuda
    IF v_monto IS NULL THEN
        RAISE EXCEPTION 'La deuda no existe';
    ELSIF v_saldo < v_monto THEN
        RAISE EXCEPTION 'No se puede eliminar una deuda que ya tiene pagos';
    END IF;
	-- Eliminar la deuda
    DELETE FROM deudas WHERE deuda_id = p_deuda_id;
END;
$$ LANGUAGE plpgsql;

-- Funcion para consultar deuda
CREATE OR REPLACE FUNCTION fn_deuda_consultar(
    p_deuda_id INT
) RETURNS TABLE (
    deuda_id INT,
    deudor VARCHAR,
    acreedor VARCHAR,
    monto NUMERIC,
    saldo NUMERIC,
    estado VARCHAR,
    fecha_creacion TIMESTAMP
) AS 
$$
BEGIN
    RETURN QUERY
    SELECT d.deuda_id,
           u1.nombre AS deudor,
           u2.nombre AS acreedor,
           d.monto,
           d.saldo,
           d.estado,
           d.fecha_creacion
    FROM deudas d
    JOIN usuarios u1 ON u1.usuario_id = d.deudor_id
    JOIN usuarios u2 ON u2.usuario_id = d.acreedor_id
    WHERE d.deuda_id = p_deuda_id;
END;
$$ LANGUAGE plpgsql;


-------- (Pagos) ------------
-- Funcion para registrar un pago
CREATE OR REPLACE FUNCTION fn_pago_crear(
    p_deuda_id INT,
    p_monto NUMERIC
) RETURNS VOID AS $$
DECLARE
    v_saldo NUMERIC;
BEGIN
    -- Verificar saldo actual
    SELECT saldo INTO v_saldo FROM deudas WHERE deuda_id = p_deuda_id;

    IF v_saldo IS NULL THEN
        RAISE EXCEPTION 'La deuda no existe';
    ELSIF v_saldo = 0 THEN
        RAISE EXCEPTION 'La deuda ya está pagada';
    ELSIF p_monto > v_saldo THEN
        RAISE EXCEPTION 'El monto a pagar excede el saldo de la deuda';
    END IF;

    -- Registrar el pago
    INSERT INTO pagos (deuda_id, monto) VALUES (p_deuda_id, p_monto);

    -- Actualizar saldo de la deuda
    UPDATE deudas
    SET saldo = saldo - p_monto,
        estado = CASE WHEN saldo - p_monto = 0 THEN 'PAGADA' ELSE 'PENDIENTE' END
    WHERE deuda_id = p_deuda_id;
END;
$$ LANGUAGE plpgsql;


-- Funcion para editar pago
CREATE OR REPLACE FUNCTION fn_pago_editar(
    p_pago_id INT,
    p_monto NUMERIC
) RETURNS VOID AS
$$
DECLARE
    v_deuda_id INT;
    v_monto_anterior NUMERIC;
BEGIN
    -- Obtener datos del pago
    SELECT deuda_id, monto INTO v_deuda_id, v_monto_anterior
    FROM pagos
    WHERE pago_id = p_pago_id;

    IF v_deuda_id IS NULL THEN
        RAISE EXCEPTION 'El pago no existe';
    END IF;

    -- Revertir efecto del pago anterior
    UPDATE deudas
    SET saldo = saldo + v_monto_anterior
    WHERE deuda_id = v_deuda_id;

    -- Validar nuevo monto
    IF (SELECT saldo FROM deudas WHERE deuda_id = v_deuda_id) < p_monto THEN
        RAISE EXCEPTION 'El nuevo monto excede el saldo disponible de la deuda';
    END IF;

    -- Actualizar el pago
    UPDATE pagos
    SET monto = p_monto
    WHERE pago_id = p_pago_id;

    -- Aplicar el nuevo pago
    UPDATE deudas
    SET saldo = saldo - p_monto,
        estado = CASE WHEN saldo - p_monto = 0 THEN 'PAGADA' ELSE 'PENDIENTE' END
    WHERE deuda_id = v_deuda_id;
END;
$$ LANGUAGE plpgsql;

-- Funcion para eliminar pago
CREATE OR REPLACE FUNCTION fn_pago_eliminar(
    p_pago_id INT
) RETURNS VOID AS
$$
DECLARE
    v_deuda_id INT;
    v_monto NUMERIC;
BEGIN
    -- Obtener datos del pago
    SELECT deuda_id, monto INTO v_deuda_id, v_monto
    FROM pagos
    WHERE pago_id = p_pago_id;

    IF v_deuda_id IS NULL THEN
        RAISE EXCEPTION 'El pago no existe';
    END IF;

    -- Eliminar el pago
    DELETE FROM pagos WHERE pago_id = p_pago_id;

    -- Revertir saldo en la deuda
    UPDATE deudas
    SET saldo = saldo + v_monto,
        estado = 'PENDIENTE'
    WHERE deuda_id = v_deuda_id;
END;
$$ LANGUAGE plpgsql;

-- Funcion para consultar un pago
CREATE OR REPLACE FUNCTION fn_pago_consultar(
    p_pago_id INT
) RETURNS TABLE (
    pago_id INT,
    deuda_id INT,
    monto NUMERIC,
    fecha_pago TIMESTAMP
) AS
$$
BEGIN
    RETURN QUERY
    SELECT p.pago_id, p.deuda_id, p.monto, p.fecha_pago
    FROM pagos p
    WHERE p.pago_id = p_pago_id;
END;
$$ LANGUAGE plpgsql;



-------------- (Queries Personalizados) -------------------
-- Funcion para listar pagos de una deuda
CREATE OR REPLACE FUNCTION fn_pago_listar(
    p_deuda_id INT
) RETURNS TABLE (
    pago_id INT,
    monto NUMERIC,
    fecha_pago TIMESTAMP
) AS
$$
BEGIN
    RETURN QUERY
    SELECT p.pago_id, p.monto, p.fecha_pago
    FROM pagos p
    WHERE p.deuda_id = p_deuda_id
    ORDER BY p.fecha_pago;
END;
$$ LANGUAGE plpgsql;

-- Funcion para listar las deudas de un usuario (pendientes y pagadas)
CREATE OR REPLACE FUNCTION fn_deuda_listar_usuario(
    p_usuario_id INT
) RETURNS TABLE (
    deuda_id INT,
    deudor VARCHAR,
    acreedor VARCHAR,
    monto NUMERIC,
    saldo NUMERIC,
    estado VARCHAR,
    fecha_creacion TIMESTAMP
) AS
$$
BEGIN
    RETURN QUERY
    SELECT d.deuda_id,
           u1.nombre AS deudor,
           u2.nombre AS acreedor,
           d.monto,
           d.saldo,
           d.estado,
           d.fecha_creacion
    FROM deudas d
    JOIN usuarios u1 ON u1.usuario_id = d.deudor_id
    JOIN usuarios u2 ON u2.usuario_id = d.acreedor_id
    WHERE d.deudor_id = p_usuario_id OR d.acreedor_id = p_usuario_id
    ORDER BY d.fecha_creacion DESC;
END;
$$ LANGUAGE plpgsql;



