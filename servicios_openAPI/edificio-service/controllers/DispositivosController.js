/**
 * The DispositivosController file is a very simple one, which does not need to be changed manually,
 * unless there's a case where business logic routes the request to an entity which is not
 * the service.
 * The heavy lifting of the Controller item is done in Request.js - that is where request
 * parameters are extracted and sent to the service, and where response is handled.
 */

const Controller = require('./Controller');
const service = require('../services/DispositivosService');
const dispositivoCodigoDELETE = async (request, response) => {
  await Controller.handleRequest(request, response, service.dispositivoCodigoDELETE);
};

const dispositivoCodigoGET = async (request, response) => {
  await Controller.handleRequest(request, response, service.dispositivoCodigoGET);
};

const dispositivoCodigoPUT = async (request, response) => {
  await Controller.handleRequest(request, response, service.dispositivoCodigoPUT);
};

const dispositivoPOST = async (request, response) => {
  await Controller.handleRequest(request, response, service.dispositivoPOST);
};


module.exports = {
  dispositivoCodigoDELETE,
  dispositivoCodigoGET,
  dispositivoCodigoPUT,
  dispositivoPOST,
};
