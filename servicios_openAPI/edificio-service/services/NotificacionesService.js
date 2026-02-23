/* eslint-disable no-unused-vars */
const Service = require('./Service');
const mysql = require('mysql2/promise');
const nodemailer = require('nodemailer');

const pool = mysql.createPool({
  host: 'localhost',
  port: 3307,
  user: 'root',
  password: 'root',
  database: 'practica1'
});

const transporter = nodemailer.createTransport({
  host: 'localhost',
  port: 1025,
  secure: false,
});

/**
* Notificar un error a un empleado por email
*
* wsKey String 
* notificacionError NotificacionError 
* no response value expected for this operation
* */
const notificacionesErrorPOST = ({ wsKey, body }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keyResult] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keyResult.length === 0) {
        return resolve(Service.rejectResponse('WSKey inválida', 401));
      }

      const { nifnie, error } = body;

      const [result] = await pool.query('SELECT email FROM empleados WHERE nifnie = ?', [nifnie]);
      if (result.length === 0) {
        return resolve(Service.rejectResponse('Empleado no encontrado', 404));
      }

      const email = result[0].email;
      await transporter.sendMail({
        from: 'noreply@empresa.com',
        to: email,
        subject: 'Notificación de Error',
        text: `Se ha producido el siguiente error: ${error}`,
      });

      resolve(Service.successResponse({ message: 'Notificación de error enviada correctamente' }));

    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);

/**
* Notificar presencia en sala por email
*
* wsKey String 
* nody NotificacionPresencia 
* no response value expected for this operation
* */
const notificacionesPresenciaPOST = ({ wsKey, body }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keyResult] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keyResult.length === 0) {
        return resolve(Service.rejectResponse('WSKey inválida', 401));
      }

      const { codigoSala } = body;

      const [salaResult] = await pool.query('SELECT nombre FROM salas WHERE codigoSala = ?', [codigoSala]);
      if (salaResult.length === 0) {
        return resolve(Service.rejectResponse('Sala no encontrada', 404));
      }

      const nombreSala = salaResult[0].nombre;
      const [empleados] = await pool.query(
        'SELECT e.email FROM controlpresencia cp JOIN empleados e ON cp.idEmpleado = e.id JOIN salas s ON cp.idSala = s.id WHERE s.codigoSala = ?',
        [codigoSala]
      );

      if (empleados.length === 0) {
        return resolve(Service.rejectResponse('No hay empleados presentes en la sala', 404));
      }

      const emailList = empleados.map(emp => emp.email).join(',');
      await transporter.sendMail({
        from: 'noreply@empresa.com',
        to: emailList,
        subject: 'Notificación de Presencia en Sala',
        text: `Se ha registrado su presencia en la sala: ${nombreSala}`,
      });

      resolve(Service.successResponse({ message: 'Notificación de presencia enviada correctamente' }));

    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);
/**
* Notificar si un empleado es válido por email
*
* wsKey String 
* body NotificacionUsuario 
* no response value expected for this operation
* */
const notificacionesUsuarioValidoPOST = ({ wsKey, body }) => new Promise(
  async (resolve, reject) => {
    try {
      const [keyResult] = await pool.query('SELECT * FROM restkey WHERE rest_key = ?', [wsKey]);
      if (keyResult.length === 0) {
        return resolve(Service.rejectResponse('WSKey invalida', 401));
      }

      const { nifnie } = body;

      const [result] = await pool.query('SELECT email, valido FROM empleados WHERE nifnie = ?', [nifnie]);
      if (result.length === 0) {
        return resolve(Service.rejectResponse('Empleado no encontrado', 404));
      }

      const email = result[0].email;
      const esValido = result[0].valido === 1 ? 'válido' : 'no válido';
      await transporter.sendMail({
        from: 'noreply@empresa.com',
        to: email,
        subject: 'Notificación de Validez del Usuario',
        text: `Su usuario con NIF/NIE ${nifnie} es ${esValido} en el sistema.`,
      });

      resolve(Service.successResponse({ message: 'Notificación de usuario válido enviada correctamente' }));

    } catch (e) {
      reject(Service.rejectResponse(
        e.message || 'Invalid input',
        e.status || 405,
      ));
    }
  },
);

module.exports = {
  notificacionesErrorPOST,
  notificacionesPresenciaPOST,
  notificacionesUsuarioValidoPOST,
};
