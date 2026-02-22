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
* wskey String 
* returns Sala
* */
const salasCodigoSalaGET = ({ codigoSala, wskey }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        codigoSala,
        wskey,
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
* codigoSala Integer 
* sala Sala 
* no response value expected for this operation
* */
const salasCodigoSalaPUT = ({ codigoSala, sala }) => new Promise(
  async (resolve, reject) => {
    try {
      resolve(Service.successResponse({
        codigoSala,
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
  salasCodigoSalaPUT,
  salasPOST,
};
