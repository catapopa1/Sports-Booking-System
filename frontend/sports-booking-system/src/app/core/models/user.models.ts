export type UserRole = 'Admin' | 'ParkManager' | 'Player';

export interface UserProfileDto {
  userId: number;
  fullName: string;
  bio: string | null;
  profilePictureUrl: string | null;
  role: string;
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
