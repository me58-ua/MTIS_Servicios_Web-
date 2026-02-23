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
* Borrar nivel por su nivel
*
* nivel Integer 
* wsKey String 
* no response value expected for this operation
* */
const nivelNivelDELETE = ({ nivel, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query('DELETE FROM niveles WHERE nivel = ?', [nivel]);
      if (result.affectedRows === 0) {
        return resolve(Service.rejectResponse('Nivel no encontrado', 404));
      }

      resolve(Service.successResponse({ message: 'Nivel eliminado correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Consultar nivel por su nivel
*
* nivel Integer 
* wsKey String 
* returns Nivel
* */
const nivelNivelGET = ({ nivel, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query('SELECT * FROM niveles WHERE nivel = ?', [nivel]);
      if (result.length === 0) {
        return resolve(Service.rejectResponse('Nivel no encontrado', 404));
      }

      resolve(Service.successResponse(result[0]));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Modificar nivel completo
*
* nivel Integer 
* body Nivel 
* no response value expected for this operation
* */
const nivelNivelPUT = ({ nivel, body }) => new Promise(
  async (resolve, reject) => {
    try {
      const { nivel: newNivel, descripcion, wsKey } = body;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query(
        'UPDATE niveles SET nivel = ?, descripcion = ? WHERE nivel = ?',
        [newNivel, descripcion, nivel]
      );
      if (result.affectedRows === 0) {
        return resolve(Service.rejectResponse('Nivel no encontrado', 404));
      }

      resolve(Service.successResponse({ message: 'Nivel actualizado correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Crear nuevo nivel
*
* nivel Nivel 
* no response value expected for this operation
* */
const nivelPOST = ({ body }) => new Promise(
  async (resolve, reject) => {
    try {
      const { nivel, descripcion, wsKey } = body;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      await pool.query(
        'INSERT INTO niveles (nivel, descripcion) VALUES (?, ?)',
        [nivel, descripcion]
      );

      resolve(Service.successResponse({ message: 'Nivel creado correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);

module.exports = {
  nivelNivelDELETE,
  nivelNivelGET,
  nivelNivelPUT,
  nivelPOST,
};
