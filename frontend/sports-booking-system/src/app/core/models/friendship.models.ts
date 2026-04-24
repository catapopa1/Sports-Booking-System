export type FriendshipStatus = 'Requested' | 'Accepted' | 'Rejected' | 'Blocked';

export interface FriendDto {
  userId: number;
  fullName: string;
  profilePictureUrl: string | null;
}

export interface FriendRequestDto {
  friendshipId: number;
  userId: number;
  fullName: string;
  profilePictureUrl: string | null;
  createdAt: string;
}