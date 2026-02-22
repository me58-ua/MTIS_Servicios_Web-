/* eslint-disable no-unused-vars */
const Service = require('./Service');

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
      resolve(Service.successResponse({
        codigo,
        wsKey,
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
* Consultar dispositivo por su codigo
*
* codigo Integer 
* wsKey String 
* returns Dispositivo
* */
const dispositivoCodigoGET = ({ codigo, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        codigo,
        wsKey,
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
* Modificar dispositivo por id
*
* id Integer 
* dispositivo Dispositivo 
* no response value expected for this operation
* */
const dispositivoIdPUT = ({ id, dispositivo }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        id,
        dispositivo,
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
* Crear nuevo dispositivo
*
* dispositivo Dispositivo 
* no response value expected for this operation
* */
const dispositivoPOST = ({ dispositivo }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        dispositivo,
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
  dispositivoCodigoDELETE,
  dispositivoCodigoGET,
  dispositivoIdPUT,
  dispositivoPOST,
};
