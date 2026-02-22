/* eslint-disable no-unused-vars */
const Service = require('./Service');
const mysql = require('mysql2/promise')

const pool = mysql.createPool({
  host: 'localhost',
  user: 'root',
  password: 'root',
  database: 'practica1'
});

/**
* Borrar sala por su codigo
*
* codigoSala Integer 
* wsKey String 
* no response value expected for this operation
* */
const salasCodigoSalaDELETE = ({ codigoSala, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query('DELETE FROM salas WHERE codigoSala = ?', [codigoSala]);
      if (result.affectedRows === 0) {
        return resolve(Service.rejectResponse('Sala no encontrada', 404));
      }

      resolve(Service.successResponse({ message: 'Sala eliminada correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Consultar sala pr codigo
*
* codigoSala Integer 
* wskey String 
* returns Sala
* */
const salasCodigoSalaGET = ({ codigoSala, wskey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wskey]);
      if(keys.length === 0){
        return resolve(Service.rejectResponse('Wskey invalida', 401));
      }
      const [result] = await pool.query('SELECT * FROM salas WHERE codigoSala = ?', [codigoSala]);
      if(result.length === 0){
        return resolve(Service.rejectResponse('Sala no encontrada', 404));
      }
      resolve(Service.successResponse(
        result[0]
      ));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Modificar datos de Sala
*
* id Integer 
* sala Sala 
* no response value expected for this operation
* */
const salasIdPUT = ({ id, sala }) => new Promise(
  async (resolve, reject) => {
    try {
      const { codigoSala, nombre, nivel, wsKey } = sala;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey inválida', 401));
      }

      const [result] = await pool.query(
        'UPDATE salas SET codigoSala = ?, nombre = ?, nivel = ? WHERE id = ?',
        [codigoSala, nombre, nivel, id]
      );
      if (result.affectedRows === 0) {
        return resolve(Service.rejectResponse('Sala no encontrada', 404));
      }

      resolve(Service.successResponse({ message: 'Sala actualizada correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Crear nueva sala
*
* sala Sala 
* no response value expected for this operation
* */
const salasPOST = ({ sala }) => new Promise(
  async (resolve, reject) => {
    try {
      const { codigoSala, nombre, nivel, wsKey } = sala;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey inválida', 401));
      }

      await pool.query(
        'INSERT INTO salas (codigoSala, nombre, nivel) VALUES (?, ?, ?)',
        [codigoSala, nombre, nivel]
      );

      resolve(Service.successResponse({ message: 'Sala creada correctamente' }, 201));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);

module.exports = {
  salasCodigoSalaDELETE,
  salasCodigoSalaGET,
  salasIdPUT,
  salasPOST,
};
