/* eslint-disable no-unused-vars */
const Service = require('./Service');
const mysql = require('mysql2/promise');

const pool = mysql.createPool({
  host: 'localhost',
  port: 3307,
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
* wsKey String 
* returns Sala
* */
const salasCodigoSalaGET = ({ codigoSala, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if(keys.length === 0){
        return resolve(Service.rejectResponse('WSKey invalida', 401));
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
* codigoSala Integer 
* body Sala 
* no response value expected for this operation
*/
const salasCodigoSalaPUT = ({ codigoSala, body }) => new Promise(
  async (resolve, reject) => {
    try {
      const { codigoSala: newCodigoSala, nombre, nivel, wsKey } = body;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey inválida', 401));
      }

      const [result] = await pool.query(
        'UPDATE salas SET codigoSala = ?, nombre = ?, nivel = ? WHERE codigoSala = ?',
        [newCodigoSala, nombre, nivel, codigoSala]
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
* body Sala 
* no response value expected for this operation
*/
const salasPOST = ({ body }) => new Promise(
  async (resolve, reject) => {
    try {
      const { codigoSala, nombre, nivel, wsKey } = body;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      // Check if the nivel exists
      const [niveles] = await pool.query('SELECT * FROM niveles WHERE nivel = ?', [nivel]);
      if (niveles.length === 0) {
        return resolve(Service.rejectResponse('No se puede crear sala porque no existe el nivel', 400));
      }

      await pool.query(
        'INSERT INTO salas (codigoSala, nombre, nivel) VALUES (?, ?, ?)',
        [codigoSala, nombre, nivel]
      );

      resolve(Service.successResponse({ message: 'Sala creada correctamente' }));

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
  salasCodigoSalaPUT,
  salasPOST,
};
