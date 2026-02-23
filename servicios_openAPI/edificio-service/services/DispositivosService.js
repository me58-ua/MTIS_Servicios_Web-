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
* Borrar dispositivo por su código
*
* codigo Integer 
* wsKey String 
* no response value expected for this operation
* */
const dispositivoCodigoDELETE = ({ codigo, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query('DELETE FROM dispositivo WHERE codigo = ?', [codigo]);
      if (result.affectedRows === 0) {
        return resolve(Service.rejectResponse('Dispositivo no encontrado', 404));
      }

      resolve(Service.successResponse({ message: 'Dispositivo eliminado correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Consultar dispositivo por su codigo
*
* codigo Integer 
* wsKey String 
* returns Dispositivo
* */
const dispositivoCodigoGET = ({ codigo, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query('SELECT * FROM dispositivo WHERE codigo = ?', [codigo]);
      if (result.length === 0) {
        return resolve(Service.rejectResponse('Dispositivo no encontrado', 404));
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
* Modificar dispositivo
*
* codigo Integer 
* body Dispositivo 
* no response value expected for this operation
* */
const dispositivoCodigoPUT = ({ codigo, body }) => new Promise(
  async (resolve, reject) => {
    try {
      const { codigo: newCodigo, descripcion, wsKey } = body;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const [result] = await pool.query(
        'UPDATE dispositivo SET codigo = ?, descripcion = ? WHERE codigo = ?',
        [newCodigo, descripcion, codigo]
      );
      if (result.affectedRows === 0) {
        return resolve(Service.rejectResponse('Dispositivo no encontrado', 404));
      }

      resolve(Service.successResponse({ message: 'Dispositivo actualizado correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Crear nuevo dispositivo
*
* dispositivo Dispositivo 
* no response value expected for this operation
* */
const dispositivoPOST = ({ dispositivo }) => new Promise(
  async (resolve, reject) => {
    try {
      const { codigo, descripcion, wsKey } = dispositivo;

      const [keys] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keys.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      await pool.query(
        'INSERT INTO dispositivo (codigo, descripcion) VALUES (?, ?)',
        [codigo, descripcion]
      );

      resolve(Service.successResponse({ message: 'Dispositivo creado correctamente' }));
    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);

module.exports = {
  dispositivoCodigoDELETE,
  dispositivoCodigoGET,
  dispositivoCodigoPUT,
  dispositivoPOST,
};
