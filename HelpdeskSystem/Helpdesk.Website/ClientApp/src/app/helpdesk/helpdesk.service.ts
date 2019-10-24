import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { GetHelpdesksResponse } from '../data/responses/helpdesk/get-all-response';
import { GetHelpdeskResponse } from '../data/responses/configuration/get-response';
import { GetUnitsByHelpdeskIdResponse } from '../data/responses/units/get-by-help-id.response';
import { CheckInRequest } from '../data/requests/check-in/chek-in-request';
import { CheckInResponse } from '../data/responses/helpdesk/check-in.response';
import { GetCheckInsResponse } from '../data/responses/helpdesk/get-check-ins.response';
import { CheckOutRequest } from '../data/requests/check-in/check-out-request';
import { GetQueueItemsByHelpdeskIDResponse } from '../data/responses/helpdesk/get-queue-items-by-helpdesk-response';
import { AddToQueueRequest } from '../data/requests/queue/add-to-queue-request';
import { AddToQueueResponse } from '../data/responses/helpdesk/add-to-queue-response';
import { UpdateQueueItemStatusRequest } from '../data/requests/queue/update-queue-item-status-request';
import { UpdateQueueItemRequest } from '../data/requests/queue/update-queue-item-request';
import { UpdateQueueItemResponse } from '../data/responses/helpdesk/update-queue-item-response';

@Injectable()
/**
 * Used to call the CRUD APIs for helpdesks
 */
export class HelpdeskService {

  constructor(private client: HttpClient) {
  }

  /**
   * Returns all helpdesks
   * @return Helpdesk[]
   */
  getHelpdesks() {
    return this.client.get<GetHelpdesksResponse>('/api/helpdesk/');
  }

  /**
   * Returns all active helpdesks
   * @return Helpdesk[]
   */
  getActiveHelpdesks() {
    return this.client.get<GetHelpdesksResponse>('/api/helpdesk/active');
  }

  /**
   * Returns a specific helpdesk
   * @param id Unique Id of the helpdesk to return
   * @return GetHelpdeskResponse
   */
  getHelpdeskById(id: number) {
    return this.client.get<GetHelpdeskResponse>('/api/helpdesk/' + id);
  }

  /**
   * Used to retreive the list of active units for the helpdesk
   * @param id The id of the helpdesk
   * @returns GetUnitsByHelpdeskIdResponse
   */
  getActiveUnitsByHelpdeskId(id: number) {
    return this.client.get<GetUnitsByHelpdeskIdResponse>('/api/units/helpdesk/' + id + '/active');
  }

  /**
   * This function is used to check in a student
   * @param request the check in information
   * @returns the response that indicates success
   */
  checkIn(request: CheckInRequest) {
    return this.client.post<CheckInResponse>('/api/checkin/', request);
  }

  /**
   * This function is used to check out a student
   * @param id the check in id
   * @param request request indicating the the checkout is not forced
   */
  checkOut(id: number, request: CheckOutRequest) {
    return this.client.post<CheckInResponse>('/api/checkin/' + id , request);
  }

  /**
   * Used to get the check ins for the helpdesk id required
   * @param id The id of the helpdesk
   * @returns The response with the check ins if they are any
   */
  getCheckInsByHelpdesk(id: number) {
    return this.client.get<GetCheckInsResponse>('/api/checkin/' + id);
  }

  /**
   * Used to retreive all active queue items for a helpdesk
   * @param id the id of the helpdesk
   * @returns the response that contains an array of helpdesk QueueItems
   */
  getQueueItemsByHelpdesk(id: number) {
    return this.client.get<GetQueueItemsByHelpdeskIDResponse>('/api/queue/helpdesk/' + id);
  }

  /**
   * Used to join the queue for a particular helpdesk
   * @param request the student information for the queue item
   */
  addToQueue(request: AddToQueueRequest) {
    return this.client.post<AddToQueueResponse>('/api/queue', request);
  }

/**
 * Used to call the the update Queue item status API
 * @param id the id of the queue item
 * @param request the information for the status update
 */
  updateQueueItemStatus(id: number, request: UpdateQueueItemStatusRequest) {
    return this.client.post<UpdateQueueItemStatusRequest>('/api/queue/' + id + '/UpdateQueueItemStatus', request);
  }

  /**
   * Used to call the API to update and edited queue item
   * @param id the id of the item to be updated
   * @param request the updated information for the queue item
   */
  updateQueueItem(id: number, request: UpdateQueueItemRequest) {
    return this.client.post<UpdateQueueItemResponse>('/api/queue/' + id, request);
  }
}
