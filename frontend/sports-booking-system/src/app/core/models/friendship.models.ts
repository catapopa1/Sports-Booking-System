export type FriendshipStatus = 'Requested' | 'Accepted' | 'Rejected' | 'Blocked';

export interface FriendDto {
  friendshipId: number;
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