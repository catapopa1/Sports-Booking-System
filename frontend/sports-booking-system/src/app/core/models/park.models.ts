export type SportType = 'Football' | 'Tennis' | 'Basketball'

export interface ParkSummaryDto {
  id: number;
  name: string;
  city: string;
  fieldCount: number;
}

export interface ParkDto {
  id: number;
  name: string;
  address: string;
  city: string;
  managerId: number;
  managerName: string;
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