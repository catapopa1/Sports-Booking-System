export type BookingType = 'Standard' | 'FullCourt' | 'HalfCourt';
export type BookingStatus = 'Requested' | 'PendingPlayerConfirmations' | 'PendingManagerApproval' | 'Confirmed' | 'Cancelled' | 'TimedOut';
export type InviteStatus = 'Pending' | 'Accepted' | 'Declined';

export interface InviteDto {
  playerId: number;
  playerName: string;
  status: string;
}

export interface BookingDto {
  id: number;
  fieldId: number;
  fieldName: string;
  parkName: string;
  startTime: string;
  bookingType: string;
  status: string;
  totalPrice: number;
  requiredPlayerCount: number;
  invites: InviteDto[];
}

export interface BookingSummaryDto {
  id: number;
  fieldName: string;
  parkName: string;
  startTime: string;
  status: string;
  totalPrice: number;
}

export interface InviteNotificationDto {
  bookingId: number;
  organizerName: string;
  fieldName: string;
  parkName: string;
  startTime: string;
  inviteStatus: string;
}

export interface CreateBookingRequest {
  fieldId: number;
  startDate: string;
  bookingType: BookingType;
  invitedPlayersIds: number[];
}