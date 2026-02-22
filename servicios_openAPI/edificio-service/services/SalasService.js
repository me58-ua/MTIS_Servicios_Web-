/* eslint-disable no-unused-vars */
const Service = require('./Service');

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
      resolve(Service.successResponse({
        codigoSala,
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
* Consultar sala pr codigo
*
* codigoSala Integer 
* wsKey String 
* returns Sala
* */
const salasCodigoSalaGET = ({ codigoSala, wsKey }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        codigoSala,
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
* Modificar datos de Sala
*
* id Integer 
* sala Sala 
* no response value expected for this operation
* */
const salasIdPUT = ({ id, sala }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        id,
        sala,
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
* Crear nueva sala
*
* sala Sala 
* no response value expected for this operation
* */
const salasPOST = ({ sala }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        sala,
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
  salasCodigoSalaDELETE,
  salasCodigoSalaGET,
  salasIdPUT,
  salasPOST,
};
