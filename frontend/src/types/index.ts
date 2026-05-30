export enum UserRole {
  SuperAdmin = 1,
  Admin = 2,
  User = 3,
}

export enum MotorStatus {
  Inactive = 0,
  Active = 1,
  Running = 2,
  Maintenance = 3,
  Fault = 4,
}

export enum MotorState {
  Off = 0,
  On = 1,
}

export enum NotificationType {
  Info = 0,
  Warning = 1,
  Critical = 2,
  Success = 3,
}

export interface User {
  id: string;
  name: string;
  mobileNumber: string;
  email?: string;
  role: UserRole;
  isActive: boolean;
  isLocked?: boolean;
  centerId?: string;
  centerName?: string;
  lastLoginAt?: string;
  profileImageUrl?: string;
  createdAt?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: User;
}

export interface Center {
  id: string;
  name: string;
  description?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  contactPhone?: string;
  contactEmail?: string;
  isActive: boolean;
  locationCount: number;
  motorCount: number;
  createdAt: string;
  requiresAssignment: boolean;
}

export interface Location {
  id: string;
  centerId: string;
  centerName: string;
  name: string;
  description?: string;
  floor?: string;
  zone?: string;
  isActive: boolean;
  motorCount: number;
  activeMotorCount: number;
  createdAt: string;
}

export interface Motor {
  id: string;
  locationId: string;
  locationName: string;
  centerId: string;
  centerName: string;
  motorNumber: string;
  description?: string;
  waterCapacityLiters: number;
  status: MotorStatus;
  currentState: MotorState;
  lastOpenedAt?: string;
  lastClosedAt?: string;
  totalRunningMinutes: number;
  isActive: boolean;
  assignedSewadaarId?: string;
  assignedSewadaarName?: string;
  createdAt: string;
}

export interface MotorLog {
  id: string;
  motorId: string;
  motorNumber: string;
  operatedByUserId: string;
  operatedByUserName: string;
  action: 'Open' | 'Close';
  actionTime: string;
  durationMinutes?: number;
  notes?: string;
}

export interface Notification {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
  isRead: boolean;
  readAt?: string;
  createdAt: string;
}

export interface AuditLog {
  id: string;
  userId?: string;
  userName?: string;
  action: string;
  entityType: string;
  entityId?: string;
  oldValues?: string;
  newValues?: string;
  ipAddress?: string;
  timestamp: string;
}

export interface DashboardSummary {
  totalCenters: number;
  totalLocations: number;
  totalMotors: number;
  totalUsers: number;
  activeMotors: number;
  runningMotors: number;
  faultMotors: number;
  totalSewadars: number;
  totalRunningMinutesToday: number;
}

export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ApiResponse<T = void> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}

export interface QueryParams {
  pageNumber?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  isActive?: boolean;
}

export enum AssignmentStatus {
  Active = 0,
  Inactive = 1,
  Revoked = 2,
}

export interface Assignment {
  id: string;
  userId: string;
  userName: string;
  userMobile: string;
  centerId: string;
  centerName: string;
  locationId?: string;
  locationName?: string;
  motorId?: string;
  motorNumber?: string;
  status: AssignmentStatus;
  assignedAt: string;
  revokedAt?: string;
  notes?: string;
  createdAt: string;
}
