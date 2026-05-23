export type SportType = 'Football' | 'Tennis' | 'Basketball'

export interface ParkPhotoDto {
  id: number;
  url: string;
  isMain: boolean;
  orderIndex: number;
}

export interface ParkSummaryDto {
  id: number;
  name: string;
  city: string;
  fieldCount: number;
  mainPhotoUrl: string | null;
}

export interface ParkDto {
  id: number;
  name: string;
  address: string;
  city: string;
  managerId: number;
  managerName: string;
  photos: ParkPhotoDto[];
}

export interface FieldDto {
  id: number;
  name: string;
  sportType: string;
  baseHourlyPrice: number;
  parkId: number;
  parkName: string;
}

export interface ParkStatsDto {
  parkId: number;
  parkName: string;
  totalConfirmedBookings: number;
  totalUniquePlayers: number;
  mostPlayedSport: string | null;
  busiestHour: number | null;
  busiestWeekday: string | null;
  topOrganizers: { userId: number; fullName: string; bookingCount: number }[];
}