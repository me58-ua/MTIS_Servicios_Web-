/* eslint-disable no-unused-vars */
const Service = require('./Service');

/**
* Notificar un error a un empleado por email
*
* notificacionError NotificacionError 
* no response value expected for this operation
* */
const notificacionesErrorPOST = ({ notificacionError }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        notificacionError,
      }));
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
* notificacionPresencia NotificacionPresencia 
* no response value expected for this operation
* */
const notificacionesPresenciaPOST = ({ notificacionPresencia }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        notificacionPresencia,
      }));
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
* notificacionUsuario NotificacionUsuario 
* no response value expected for this operation
* */
const notificacionesUsuarioValidoPOST = ({ notificacionUsuario }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        notificacionUsuario,
      }));
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
