export type UserRole = 'Admin' | 'ParkManager' | 'Player';

export type SportType = 'Football' | 'Tennis' | 'Basketball';
export type SportLevel = 'Beginner' | 'Intermediate' | 'Advanced' | 'Professional';

export interface UserSportSummaryDto {
  sport: SportType;
  level: SportLevel | null;
  favoriteAthlete: string | null;
  matchesPlayed: number;
}

export interface UpsertSportProfileRequest {
  level: SportLevel | null;
  favoriteAthlete: string | null;
}

export interface UserProfileDto {
  userId: number;
  fullName: string;
  bio: string | null;
  profilePictureUrl: string | null;
  role: string;
  sports: UserSportSummaryDto[];
}

export interface UserSearchResultDto {
  userId: number;
  fullName: string;
  profilePictureUrl: string | null;
  alreadyFriends: boolean;
}

export interface AdminUserDto {
  id: number;
  fullName: string;
  email: string;
  role: string;
}
