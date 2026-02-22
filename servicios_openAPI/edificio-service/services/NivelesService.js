/* eslint-disable no-unused-vars */
const Service = require('./Service');

/**
* Modificar nivel completo por su id
*
* id Integer 
* nivel Nivel 
* no response value expected for this operation
* */
const nivelIdPUT = ({ id, nivel }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        id,
        nivel,
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
* Borrar nivel por su nivel
*
* nivel Integer 
* wsKey String 
* no response value expected for this operation
* */
const nivelNivelDELETE = ({ nivel, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        nivel,
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
* Consultar nivel por su nivel
*
* nivel Integer 
* wsKey String 
* returns Nivel
* */
const nivelNivelGET = ({ nivel, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        nivel,
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
* Crear nuevo nivel
*
* nivel Nivel 
* no response value expected for this operation
* */
const nivelPOST = ({ nivel }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        nivel,
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
  nivelIdPUT,
  nivelNivelDELETE,
  nivelNivelGET,
  nivelPOST,
};
